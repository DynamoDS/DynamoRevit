﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using DSIronPython;
using Dynamo.Core.Threading;
using Dynamo.Engine;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.UpdateManager;
using Dynamo.Utilities;
using Dynamo.Core.Threading;
using DynamoServices;
using Greg;
using ProtoCore;
using Revit.Elements;
using RevitServices.Elements;

using RevitServices.Materials;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using Category = Revit.Elements.Category;
using Element = Autodesk.Revit.DB.Element;
using View = Autodesk.Revit.DB.View;

namespace Dynamo.Applications.Models
{
    public class RevitDynamoModel : DynamoModel
    {
        public interface IRevitStartConfiguration : IStartConfiguration
        {
            ExternalCommandData ExternalCommandData { get; set; }
        }

        public struct RevitStartConfiguration : IRevitStartConfiguration
        {
            public string Context { get; set; }
            public string DynamoCorePath { get; set; }
            public IPathResolver PathResolver { get; set; }
            public IPreferences Preferences { get; set; }
            public bool StartInTestMode { get; set; }
            public IUpdateManager UpdateManager { get; set; }
            public ISchedulerFactory SchedulerFactory { get; set; }
            public string GeometryFactoryPath { get; set; }
            public IAuthProvider AuthProvider { get; set; }
            public string PackageManagerAddress { get; set; }
            public ExternalCommandData ExternalCommandData { get; set; }
            public IEnumerable<Dynamo.Extensions.IExtension> Extensions { get; set; }
            public TaskProcessMode ProcessMode { get; set; }
        }

        /// <summary>
        ///     Flag for syncing up document switches between Application.DocumentClosing and
        ///     Application.DocumentClosed events.
        /// </summary>
        private bool updateCurrentUIDoc;

        private bool isFirstEvaluation = true;

        private readonly ExternalCommandData externalCommandData;

        #region Events

        public event EventHandler RevitDocumentChanged;

        public virtual void OnRevitDocumentChanged()
        {
            if (RevitDocumentChanged != null)
                RevitDocumentChanged(this, EventArgs.Empty);
        }

        public event Action RevitDocumentLost;

        private void OnRevitDocumentLost()
        {
            var handler = RevitDocumentLost;
            if (handler != null) handler();
        }

        public event Action RevitContextUnavailable;

        private void OnRevitContextUnavailable()
        {
            var handler = RevitContextUnavailable;
            if (handler != null) handler();
        }

        public event Action RevitContextAvailable;

        private void OnRevitContextAvailable()
        {
            var handler = RevitContextAvailable;
            if (handler != null) handler();
        }

        public event Action<View> RevitViewChanged;

        private void OnRevitViewChanged(View newView)
        {
            var handler = RevitViewChanged;
            if (handler != null) handler(newView);
        }

        public event Action InvalidRevitDocumentActivated;

        private void OnInvalidRevitDocumentActivated()
        {
            var handler = InvalidRevitDocumentActivated;
            if (handler != null) handler();
        }

        protected override void OnWorkspaceRemoveStarted(WorkspaceModel workspace)
        {
            base.OnWorkspaceRemoveStarted(workspace);

            if (workspace is HomeWorkspaceModel)
                DisposeLogic.IsClosingHomeworkspace = true;
        }

        protected override void OnWorkspaceAdded(WorkspaceModel workspace)
        {
            base.OnWorkspaceAdded(workspace);

            var ws = workspace as HomeWorkspaceModel;
            if (ws != null)
            {
                ws.PostTraceReconciliationComplete += PostTraceReconciliation;
            }
        }

        protected override void OnWorkspaceRemoved(WorkspaceModel workspace)
        {
            base.OnWorkspaceRemoved(workspace);

            var ws = workspace as HomeWorkspaceModel;
            if (ws != null)
            {
                DisposeLogic.IsClosingHomeworkspace = false;
                ws.PostTraceReconciliationComplete -= PostTraceReconciliation;
            }
        }

        #endregion

        #region Properties/Fields
        override internal string AppVersion
        {
            get
            {
                return base.AppVersion +
                    "-R" + DocumentManager.Instance.CurrentUIApplication.Application.VersionBuild;
            }
        }

        public bool IsInMatchingDocumentContext
        {
            get
            {
                if (DocumentManager.Instance == null)
                    return false; // Not initialized yet.

                var dm = DocumentManager.Instance;
                if (dm.CurrentDBDocument == null)
                    return false; // There's no current document stored.

                // Selection is not allowed in perspective view mode.
                var view3D = dm.CurrentUIDocument.ActiveView as View3D;
                if ((view3D != null) && view3D.IsPerspective)
                    return false; // There's no view, or in perspective view.

                // Is the cached document the same as the active document?
                var currentHashCode = dm.CurrentDBDocument.GetHashCode();
                return currentHashCode == dm.ActiveDocumentHashCode;
            }
        }
        #endregion

        #region Constructors

        public new static RevitDynamoModel Start()
        {
            return Start(new RevitStartConfiguration() { ProcessMode = TaskProcessMode.Asynchronous } );
        }

        public new static RevitDynamoModel Start(IRevitStartConfiguration configuration)
        {
            // where necessary, assign defaults
            if (string.IsNullOrEmpty(configuration.Context))
                configuration.Context = Core.Context.REVIT_2015;

            return new RevitDynamoModel(configuration);
        }

        private RevitDynamoModel(IRevitStartConfiguration configuration) :
            base(configuration)
        {
            DisposeLogic.IsShuttingDown = false;

            externalCommandData = configuration.ExternalCommandData;

            SubscribeRevitServicesUpdaterEvents();

            SubscribeApplicationEvents(configuration.ExternalCommandData);
            InitializeDocumentManager();
            SubscribeDocumentManagerEvents();
            SubscribeTransactionManagerEvents();

            MigrationManager.MigrationTargets.Add(typeof(WorkspaceMigrationsRevit));

            SetupPython();

            foreach (var ws in this.Workspaces.OfType<HomeWorkspaceModel>())
            {
                ws.PostTraceReconciliationComplete += PostTraceReconciliation;
            }
        }

        #endregion

        #region trace reconciliation

        public void PostTraceReconciliation(PostTraceReconciliationCompleteEventArgs args)
        {
            var orphanedSerializables = args.OrphanedSerializables;

            var orphanedIds =
                orphanedSerializables
                .SelectMany(kvp=>kvp.Value)
                .Cast<SerializableId>()
                .Select(sid => sid.IntID).ToList();

            if (!orphanedIds.Any())
                return;

            if (IsTestMode)
            {
                DeleteOrphanedElements(orphanedIds);
            }
            else
            {
                // Delete all the orphans.
                IdlePromise.ExecuteOnIdleAsync(
                    () =>
                    {
                        DeleteOrphanedElements(orphanedIds);
                    });
            }
        }

        private static void DeleteOrphanedElements(IEnumerable<int> orphanedIds)
        {
            var toDelete = new List<ElementId>();
            foreach (var id in orphanedIds)
            {
                // Check whether the element is valid before attempting to delete.
                Element el;
                if (DocumentManager.Instance.CurrentDBDocument.TryGetElement(new ElementId(id), out el))
                    toDelete.Add(el.Id);
            }

            if (!toDelete.Any())
                return;

            using (var trans = new Transaction(DocumentManager.Instance.CurrentDBDocument))
            {
                trans.Start("Dynamo element reconciliation.");
                DocumentManager.Instance.CurrentDBDocument.Delete(toDelete);
                trans.Commit();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// This call is made during start-up sequence after RevitDynamoModel 
        /// constructor returned. Virtual methods on DynamoModel that perform 
        /// initialization steps should only be called from here.
        /// </summary>
        internal void HandlePostInitialization()
        {
            InitializeMaterials(); // Initialize materials for preview.
        }

        private bool setupPython;

        private void SetupPython()
        {
            if (setupPython) return;

            IronPythonEvaluator.OutputMarshaler.RegisterMarshaler(
                (Element element) => element.ToDSType(true));

            // Turn off element binding during iron python script execution
            IronPythonEvaluator.EvaluationBegin +=
                (a, b, c, d, e) => ElementBinder.IsEnabled = false;
            IronPythonEvaluator.EvaluationEnd += (a, b, c, d, e) => ElementBinder.IsEnabled = true;

            // register UnwrapElement method in ironpython
            IronPythonEvaluator.EvaluationBegin += (a, b, scope, d, e) =>
            {
                var marshaler = new DataMarshaler();
                marshaler.RegisterMarshaler(
                    (Revit.Elements.Element element) => element.InternalElement);
                marshaler.RegisterMarshaler((Category element) => element.InternalCategory);

                Func<object, object> unwrap = marshaler.Marshal;
                scope.SetVariable("UnwrapElement", unwrap);
            };

            setupPython = true;
        }

        private void InitializeDocumentManager()
        {
            // Set the intitial document.
            var activeUIDocument = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            if (activeUIDocument != null)
            {
                DocumentManager.Instance.CurrentUIDocument = activeUIDocument;
                DocumentManager.Instance.HandleDocumentActivation(activeUIDocument.ActiveView);

                OnRevitDocumentChanged();
            }
        }

        private static void InitializeMaterials()
        {
            // Ensure that the current document has the needed materials
            // and graphic styles to support visualization in Revit.
            var mgr = MaterialsManager.Instance;
            IdlePromise.ExecuteOnIdleAsync(mgr.InitializeForActiveDocumentOnIdle);
        }

        #endregion

        #region Event subscribe/unsubscribe

        private void SubscribeRevitServicesUpdaterEvents()
        {
            RevitServicesUpdater.Instance.ElementsDeleted += RevitServicesUpdater_ElementsDeleted;
            RevitServicesUpdater.Instance.ElementsModified += RevitServicesUpdater_ElementsModified;
        }

        private void UnsubscribeRevitServicesUpdaterEvents()
        {
            RevitServicesUpdater.Instance.ElementsDeleted -= RevitServicesUpdater_ElementsDeleted;
            RevitServicesUpdater.Instance.ElementsModified -= RevitServicesUpdater_ElementsModified;
        }

        private void SubscribeTransactionManagerEvents()
        {
            TransactionManager.Instance.TransactionWrapper.FailuresRaised +=
                TransactionManager_FailuresRaised;
        }

        private void UnsubscribeTransactionManagerEvents()
        {
            TransactionManager.Instance.TransactionWrapper.FailuresRaised -=
                TransactionManager_FailuresRaised;
        }

        private void SubscribeDocumentManagerEvents()
        {
            DocumentManager.OnLogError += Logger.Log;
        }

        private void UnsubscribeDocumentManagerEvents()
        {
            DocumentManager.OnLogError -= Logger.Log;
        }

        private bool hasRegisteredApplicationEvents;
        private void SubscribeApplicationEvents(ExternalCommandData commandData)
        {
            if (hasRegisteredApplicationEvents)
            {
                return;
            }

            DynamoRevitApp.EventHandlerProxy.ViewActivating += OnApplicationViewActivating;
            DynamoRevitApp.EventHandlerProxy.ViewActivated += OnApplicationViewActivated;
            DynamoRevitApp.EventHandlerProxy.DocumentClosing += OnApplicationDocumentClosing;
            DynamoRevitApp.EventHandlerProxy.DocumentClosed += OnApplicationDocumentClosed;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += OnApplicationDocumentOpened;

            hasRegisteredApplicationEvents = true;
        }

        private void UnsubscribeApplicationEvents(ExternalCommandData commandData)
        {
            if (!hasRegisteredApplicationEvents)
            {
                return;
            }

            DynamoRevitApp.EventHandlerProxy.ViewActivating -= OnApplicationViewActivating;
            DynamoRevitApp.EventHandlerProxy.ViewActivated -= OnApplicationViewActivated;
            DynamoRevitApp.EventHandlerProxy.DocumentClosing -= OnApplicationDocumentClosing;
            DynamoRevitApp.EventHandlerProxy.DocumentClosed -= OnApplicationDocumentClosed;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened -= OnApplicationDocumentOpened;

            hasRegisteredApplicationEvents = false;
        }

        #endregion

        #region Application event handler
        /// <summary>
        /// Handler for Revit's DocumentOpened event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            HandleApplicationDocumentOpened();
        }

        /// <summary>
        /// Handler for Revit's DocumentClosing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            // Invalidate the cached active document value if it is the closing document.
            var activeDocumentHashCode = DocumentManager.Instance.ActiveDocumentHashCode;
            if (e.Document != null && (e.Document.GetHashCode() == activeDocumentHashCode))
                DocumentManager.Instance.HandleDocumentActivation(null);

            HandleApplicationDocumentClosing(e.Document);
        }

        /// <summary>
        /// Handler for Revit's DocumentClosed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            HandleApplicationDocumentClosed();
        }

        /// <summary>
        /// Handler for Revit's ViewActivating event.
        /// Addins are not available in some views in Revit, notably perspective views.
        /// This will present a warning that Dynamo is not available to run and disable the run button.
        /// This handler is called before the ViewActivated event registered on the RevitDynamoModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnApplicationViewActivating(object sender, ViewActivatingEventArgs e)
        {
            SetRunEnabledBasedOnContext(e.NewActiveView);
        }

        /// <summary>
        /// Handler for Revit's ViewActivated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationViewActivated(object sender, ViewActivatedEventArgs e)
        {
            HandleRevitViewActivated();
        }

        #endregion

        #region Public methods

        public override void OnEvaluationCompleted(object sender, EvaluationCompletedEventArgs e)
        {
            Debug.WriteLine(ElementIDLifecycleManager<int>.GetInstance());

            // finally close the transaction!
            TransactionManager.Instance.ForceCloseTransaction();

            base.OnEvaluationCompleted(sender, e);
        }

        protected override void PreShutdownCore(bool shutdownHost)
        {
            if (shutdownHost)
            {
                DynamoRevitApp.AddIdleAction(ShutdownRevitHostOnce);
            }

            base.PreShutdownCore(shutdownHost);
        }

        private static void ShutdownRevitHostOnce()
        {
            var uiApplication = DocumentManager.Instance.CurrentUIApplication;
            ShutdownRevitHost();
        }

        protected override void ShutDownCore(bool shutDownHost)
        {
            DisposeLogic.IsShuttingDown = true;

            base.ShutDownCore(shutDownHost);

            // unsubscribe events
            RevitServicesUpdater.Instance.UnRegisterAllChangeHooks();

            UnsubscribeApplicationEvents(externalCommandData);
            UnsubscribeDocumentManagerEvents();
            UnsubscribeRevitServicesUpdaterEvents();
            UnsubscribeTransactionManagerEvents();

            ElementIDLifecycleManager<int>.DisposeInstance();
        }

        public void SetRunEnabledBasedOnContext(View newView)
        {
            DocumentManager.Instance.HandleDocumentActivation(newView);

            var view = newView as View3D;

            if (view != null && view.IsPerspective
                && Context != Core.Context.VASARI_2014)
            {
                OnRevitContextUnavailable();

                foreach (
                    var ws in Workspaces.OfType<HomeWorkspaceModel>())
                {
                    ws.RunSettings.RunEnabled = false;
                }
            }
            else
            {
                Logger.Log(
                    string.Format("Active view is now {0}", newView.Name));

                // If there is a current document, then set the run enabled
                // state based on whether the view just activated is 
                // the same document.
                if (DocumentManager.Instance.CurrentUIDocument != null)
                {
                    var newEnabled =
                        newView.Document.Equals(DocumentManager.Instance.CurrentDBDocument);

                    if (!newEnabled)
                    {
                        OnInvalidRevitDocumentActivated();
                    }

                    foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
                    {
                        ws.RunSettings.RunEnabled = newEnabled;
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handler Revit's DocumentOpened event.
        /// It is called when a document is opened, but NOT when a document is 
        /// created from a template.
        /// </summary>
        private void HandleApplicationDocumentOpened()
        {
            // If the current document is null, for instance if there are
            // no documents open, then set the current document, and 
            // present a message telling us where Dynamo is pointing.
            if (DocumentManager.Instance.CurrentUIDocument == null)
            {
                var activeUIDocument = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                DocumentManager.Instance.CurrentUIDocument = activeUIDocument;
                if (activeUIDocument != null)
                    DocumentManager.Instance.HandleDocumentActivation(activeUIDocument.ActiveView);

                OnRevitDocumentChanged();

                foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
                {
                    ws.RunSettings.RunEnabled = true;
                }

                ResetForNewDocument();
            }
        }

        /// <summary>
        /// Handler Revit's DocumentClosing event.
        /// It is called when a document is closing.
        /// </summary>
        private void HandleApplicationDocumentClosing(Document doc)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (DocumentManager.Instance.CurrentDBDocument.Equals(doc))
            {
                updateCurrentUIDoc = true;
            }
        }

        /// <summary>
        /// Handle Revit's DocumentClosed event.
        /// It is called when a document is closed.
        /// </summary>
        private void HandleApplicationDocumentClosed()
        {
            // If the active UI document is null, it means that all views have been 
            // closed from all document. Clear our reference, present a warning,
            // and disable running.
            if (DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument == null)
            {
                DocumentManager.Instance.CurrentUIDocument = null;
                foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
                {
                    ws.RunSettings.RunEnabled = false;
                }

                OnRevitDocumentLost();
            }
            else
            {
                // If Dynamo's active UI document's document is the one that was just closed
                // then set Dynamo's active UI document to whatever revit says is active.
                if (updateCurrentUIDoc)
                {
                    updateCurrentUIDoc = false;
                    DocumentManager.Instance.CurrentUIDocument =
                        DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    OnRevitDocumentChanged();
                }
            }

            var uiDoc = DocumentManager.Instance.CurrentUIDocument;
            if (uiDoc != null)
            {
                SetRunEnabledBasedOnContext(uiDoc.ActiveView);
            }
        }

        /// <summary>
        /// Handler Revit's ViewActivated event.
        /// It is called when a view is activated. It is called after the 
        /// ViewActivating event.
        /// </summary>
        private void HandleRevitViewActivated()
        {
            // If there is no active document, then set it to whatever
            // document has just been activated
            if (DocumentManager.Instance.CurrentUIDocument == null)
            {
                DocumentManager.Instance.CurrentUIDocument =
                    DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                OnRevitDocumentChanged();

                InitializeMaterials();
                foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
                {
                    ws.RunSettings.RunEnabled = true;
                }
            }
        }

        /// <summary>
        ///     Clears all element collections on nodes and resets the visualization manager and the old value.
        /// </summary>
        private void ResetForNewDocument()
        {
            foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
            {
                ws.MarkNodesAsModifiedAndRequestRun(ws.Nodes);
            }

            OnRevitDocumentChanged();
        }

        private static void ShutdownRevitHost()
        {
            // this method cannot be called without Revit 2014
            var exitCommand = RevitCommandId.LookupPostableCommandId(PostableCommand.ExitRevit);
            var uiApplication = DocumentManager.Instance.CurrentUIApplication;

            if ((uiApplication != null) && uiApplication.CanPostCommand(exitCommand))
                uiApplication.PostCommand(exitCommand);
            else
            {
                MessageBox.Show(
                    "A command in progress prevented Dynamo from " +
                        "closing revit. Dynamo update will be cancelled.");
            }
        }

        private void TransactionManager_FailuresRaised(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failList = failuresAccessor.GetFailureMessages();

            IEnumerable<FailureMessageAccessor> query =
                from fail in failList
                where fail.GetSeverity() == FailureSeverity.Warning
                select fail;

            foreach (FailureMessageAccessor fail in query)
            {
                Logger.Log("!! Warning: " + fail.GetDescriptionText());
                failuresAccessor.DeleteWarning(fail);
            }
        }

        private void RevitServicesUpdater_ElementsDeleted(
            Document document, IEnumerable<ElementId> deleted)
        {
            if (!deleted.Any())
                return;

            foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
            {
                var nodes = ElementBinder.GetNodesFromElementIds(
                    deleted,
                    ws,
                    ws.EngineController);

                foreach (var node in nodes)
                {
                    node.OnNodeModified(forceExecute: true);
                }
            }
        }

        private void RevitServicesUpdater_ElementsModified(IEnumerable<string> updated)
        {
            var updatedIds = updated.Select(
                x =>
                {
                    Element ret;
                    DocumentManager.Instance.CurrentDBDocument.TryGetElement(x, out ret);
                    return ret;
                }).Select(x => x.Id);

            if (!updatedIds.Any())
                return;

            foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
            {
                var nodes = ElementBinder.GetNodesFromElementIds(
                    updatedIds,
                    ws,
                    ws.EngineController);

                foreach (var node in nodes)
                {
                    node.OnNodeModified(true);
                }
            }
        }

        #endregion

        protected override void OpenFileImpl(OpenFileCommand command)
        {
            DynamoRevitApp.AddIdleAction(() => base.OpenFileImpl(command));
        }

    }
}

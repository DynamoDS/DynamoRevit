using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using DSIronPython;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Interfaces;
using Dynamo.Logging;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.Updates;
using Dynamo.Utilities;
using Greg;
using Revit.Elements;
using RevitServices.Elements;

using RevitServices.Materials;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using Category = Revit.Elements.Category;
using Element = Autodesk.Revit.DB.Element;
using View = Autodesk.Revit.DB.View;
using ProtoCore;
using Dynamo.Graph.Nodes.ZeroTouch;

namespace Dynamo.Applications.Models
{
    public class RevitDynamoModel : DynamoModel
    {
        public interface IRevitStartConfiguration : IStartConfiguration
        {
            DynamoRevitCommandData ExternalCommandData { get; set; }
        }

        public struct RevitStartConfiguration : IRevitStartConfiguration
        {
            public string Context { get; set; }
            public string DynamoCorePath { get; set; }
            public string DynamoHostPath { get; set; }
            public IPathResolver PathResolver { get; set; }
            public IPreferences Preferences { get; set; }
            public bool StartInTestMode { get; set; }
            public IUpdateManager UpdateManager { get; set; }
            public ISchedulerThread SchedulerThread { get; set; }
            public string GeometryFactoryPath { get; set; }
            public IAuthProvider AuthProvider { get; set; }
            public string PackageManagerAddress { get; set; }
            public DynamoRevitCommandData ExternalCommandData { get; set; }
            public IEnumerable<Extensions.IExtension> Extensions { get; set; }
            public TaskProcessMode ProcessMode { get; set; }
        }

        /// <summary>
        ///     Flag for syncing up document switches between Application.DocumentClosing and
        ///     Application.DocumentClosed events.
        /// </summary>
        private bool updateCurrentUIDoc;

        private readonly DynamoRevitCommandData externalCommandData;

        #region Events

        /// <summary>
        /// Event triggered when the current Revit document is changed.
        /// </summary>
        public event EventHandler RevitDocumentChanged;

        public virtual void OnRevitDocumentChanged()
        {
            if (RevitDocumentChanged != null)
                RevitDocumentChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event triggered when the Revit document that Dynamo had 
        /// previously been pointing at has been closed.
        /// </summary>
        public event Action RevitDocumentLost;

        private void OnRevitDocumentLost()
        {
            var handler = RevitDocumentLost;
            if (handler != null) handler();
        }

        /// <summary>
        /// Event triggered when Revit enters a context 
        /// where external applications are not allowed.
        /// </summary>
        public event Action RevitContextUnavailable;

        private void OnRevitContextUnavailable()
        {
            var handler = RevitContextUnavailable;
            if (handler != null) handler();
        }

        /// <summary>
        /// Event triggered when Revit enters a context where
        /// external applications are allowed.
        /// </summary>
        public event Action RevitContextAvailable;

        private void OnRevitContextAvailable()
        {
            var handler = RevitContextAvailable;
            if (handler != null) handler();
        }

        /// <summary>
        /// Event triggered when the active Revit view changes.
        /// </summary>
        public event Action<View> RevitViewChanged;

        private void OnRevitViewChanged(View newView)
        {
            var handler = RevitViewChanged;
            if (handler != null) handler(newView);
        }

        /// <summary>
        /// Event triggered when a document other than the
        /// one Dynamo is pointing at becomes active.
        /// </summary>
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

        protected override void OnWorkspaceRemoved(WorkspaceModel workspace)
        {
            base.OnWorkspaceRemoved(workspace);

            if (workspace is HomeWorkspaceModel)
                DisposeLogic.IsClosingHomeworkspace = false;

            //Unsubscribe the event
            foreach (var node in workspace.Nodes.ToList())
            {
                node.PropertyChanged -= node_PropertyChanged;
            }
        }

        protected override void OnWorkspaceAdded(WorkspaceModel workspace)
        {
            base.OnWorkspaceAdded(workspace);

            foreach (var node in workspace.Nodes.ToList())
            {
                node.PropertyChanged += node_PropertyChanged;
            }

            var dm = DocumentManager.Instance;
            if (dm.CurrentDBDocument != null)
            {
                SetRunEnabledBasedOnContext(dm.CurrentUIDocument.ActiveView);
            }
        }

        private void node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsFrozen":
                    ElementBinder.SetElementFreezeState(CurrentWorkspace, sender as NodeModel, EngineController);
                    break;
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
                configuration.Context = Configuration.Context.REVIT_2015;

            return new RevitDynamoModel(configuration);
        }

        private RevitDynamoModel(IRevitStartConfiguration configuration) :
            base(configuration)
        {
            DisposeLogic.IsShuttingDown = false;

            externalCommandData = configuration.ExternalCommandData;

            RevitServicesUpdater.Initialize(DynamoRevitApp.ControlledApplication, DynamoRevitApp.Updaters);

            SubscribeRevitServicesUpdaterEvents();

            SubscribeApplicationEvents(configuration.ExternalCommandData);
            InitializeDocumentManager();
            SubscribeDocumentManagerEvents();
            SubscribeTransactionManagerEvents();

            MigrationManager.MigrationTargets.Add(typeof(WorkspaceMigrationsRevit));

            SetupPython();
        }

        private bool isFirstEvaluation = true;

        #endregion

        #region trace reconciliation

        /// <summary>
        /// This method reconciles the current state of the host with the current trace data, in revit's case
        /// deleting elements which have been orphaned and exist in trace but were not re-created
        /// </summary>
        /// <param name="orphanedSerializables"></param>
        public override void PostTraceReconciliation(Dictionary<Guid, List<ISerializable>> orphanedSerializables)
        {
            // because of multiSerialzableIDs some extra logic is 
            // required to detect if one multiSerializableID is a subset of another, if thats the case we should not delete it.

            //get all the currentTraceData again
            RuntimeCore runtimeCore = null;
            if (this.EngineController != null && (this.EngineController.LiveRunnerCore != null))
                runtimeCore = this.EngineController.LiveRunnerRuntimeCore;

            if (runtimeCore == null)
                return;

            //list of UUIDs we will remove from the list of orphans
            var toRemove = new List<String>();

            //foreach orphaned ID check 2 cases:
            //1. an orphaned MultID is totally subset in one of the latest MultIDs
            //2. the latest IDS are subset in a MultID in the orphan list
            //in either of these cases we must remove the IDs from the orphan list so they are not deleted by accident
            foreach (var orphan in orphanedSerializables)
            {
                //if there are no multiSeriializables in the orphan then we can skip this orphan
                if (orphan.Value.OfType<MultipleSerializableId>().Count() == 0)
                {
                    continue;
                }
                // Selecting all nodes that are either a DSFunction,
                // a DSVarArgFunction or a CodeBlockNodeModel into a list.
                var nodeGuids = this.Workspaces.Where(x => x.Guid == orphan.Key).First().Nodes.Where((n) =>
                {
                    return (n is DSFunction
                            || (n is DSVarArgFunction)
                            || (n is CodeBlockNodeModel));
                }).Select((n) => n.GUID);

                //this is a list of all trace data for all nodes in the workspace of this orphan
                var nodeTraceDataList = runtimeCore.RuntimeData.GetCallsitesForNodes(nodeGuids, runtimeCore.DSExecutable);
               
                //check each callsite's traceData against the orphans
                foreach (var kvp in nodeTraceDataList)
                {
                    foreach (var cs in kvp.Value)
                    {
                        var currentSerializables = cs.TraceData.SelectMany(td => td.RecursiveGetNestedData());
                        var currentStringIds = currentSerializables.OfType<MultipleSerializableId>().SelectMany(x => x.StringIDs).ToList();
                        //if the orphaned serializable exists in the currentTraceData as as subset in a MultiSerializableID
                        toRemove.AddRange(orphan.Value.OfType<MultipleSerializableId>().Where(x => currentSerializables.OfType<MultipleSerializableId>().Any(y => x.isSubset(y))).SelectMany(x=>x.StringIDs).ToList());
                        //Or if one of the callsites traceData is subset in an orphan
                        toRemove.AddRange(currentStringIds.Intersect(orphan.Value.OfType<MultipleSerializableId>().SelectMany(y => y.StringIDs)).ToList());
                        //then remove it from the orphanList so we do not delete it later

                    }
                }
            }
           
            var orphanedIds = new List<string>();
            var serializables =
                orphanedSerializables
                .SelectMany(kvp => kvp.Value)
                .Where(x => x is ISerializable);

            orphanedIds.AddRange(serializables.Where(x => x is SerializableId).
                Cast<SerializableId>().Select(sid => sid.StringID));
            orphanedIds.AddRange(serializables.Where(x => x is MultipleSerializableId).
                Cast<MultipleSerializableId>().SelectMany(sid => sid.StringIDs));

            toRemove.ForEach(x => orphanedIds.Remove(x));
            //the orphansIdsList is now free of elements that are subsets of newly created elements or Items that contain newly created elements

            if (!orphanedIds.Any())
                return;
            
            if (IsTestMode)
            {
                DeleteOrphanedElements(orphanedIds, Logger);
            }
            else
            {
                // Delete all the orphans.
                IdlePromise.ExecuteOnIdleAsync(
                    () =>
                    {
                        DeleteOrphanedElements(orphanedIds, Logger);
                    });
            }
        }

        private static void DeleteOrphanedElements(IEnumerable<string> orphanedIds, ILogger logger)
        {
            var toDelete = new List<ElementId>();
            foreach (var id in orphanedIds)
            {
                // Check whether the element is valid before attempting to delete.
                Element el;
                if (DocumentManager.Instance.CurrentDBDocument.TryGetElement(id, out el))
                    toDelete.Add(el.Id);
            }

            if (!toDelete.Any())
                return;

            using (var trans = new Transaction(DocumentManager.Instance.CurrentDBDocument))
            {
                try
                {
                    trans.Start("Dynamo element reconciliation.");
                    DocumentManager.Instance.CurrentDBDocument.Delete(toDelete);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    logger.Log(ex);
                    trans.RollBack();
                }
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
        private void SubscribeApplicationEvents(DynamoRevitCommandData commandData)
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

        private void UnsubscribeApplicationEvents(DynamoRevitCommandData commandData)
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
        private void OnApplicationDocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            HandleApplicationDocumentOpened();
        }

        /// <summary>
        /// Handler for Revit's DocumentClosing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDocumentClosing(object sender, Autodesk.Revit.DB.Events.DocumentClosingEventArgs e)
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
        private void OnApplicationDocumentClosed(object sender, Autodesk.Revit.DB.Events.DocumentClosedEventArgs e)
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
            SetRunEnabledBasedOnContext(e.NewActiveView, true);
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
                var uiApplication = DocumentManager.Instance.CurrentUIApplication;
                uiApplication.Idling += ShutdownRevitHostOnce;
            }

            base.PreShutdownCore(shutdownHost);
        }

        private static void ShutdownRevitHostOnce(object sender, IdlingEventArgs idlingEventArgs)
        {
            var uiApplication = DocumentManager.Instance.CurrentUIApplication;
            uiApplication.Idling -= ShutdownRevitHostOnce;
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

            RevitServicesUpdater.DisposeInstance();
            ElementIDLifecycleManager<int>.DisposeInstance();
        }

        /// <summary>
        /// This event handler is called if 'markNodesAsDirty' in a 
        /// prior call to RevitDynamoModel.ResetEngine was set to 'true'.
        /// </summary>
        /// <param name="markNodesAsDirty"></param>
        private void OnResetMarkNodesAsDirty(bool markNodesAsDirty)
        {
            foreach (var workspace in Workspaces.OfType<HomeWorkspaceModel>())
                workspace.ResetEngine(EngineController, markNodesAsDirty);
        }

        /// <summary>
        /// Check if the Revit context is available based on 'newView' and
        /// set the Runnsettings.RunEnabled flag on each HomeWorkspaceModel accordingly.
        /// The Revit context is unavailable for perspective views only.
        /// Raise the RevitContextAvailable event if the context is about to be available and 
        /// 'raiseRevitContextAvailableEvent' is 'true'
        /// Raise the RevitContextUnavilable event if the the context is about to be unavailable.
        /// /// </summary>
        /// <param name="newView"></param>
        /// <param name="raiseRevitContextAvailableEvent"></param>
        public void SetRunEnabledBasedOnContext(View newView, bool raiseRevitContextAvailableEvent = false)
        {
            DocumentManager.Instance.HandleDocumentActivation(newView);

            var view = newView as View3D;

            if (view != null && view.IsPerspective
                && Context != Configuration.Context.VASARI_2014)
            {
                // Pick up current state
                bool currentState = false;
                foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
                {
                    if (ws.RunSettings.RunEnabled)
                    {
                        currentState = true;
                        break;
                    }
                }

                // Only notify if we are changing state e.g. the Revit context becomes unavailable
                if (currentState)
                {
                    OnRevitContextUnavailable();
                }

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

                if(raiseRevitContextAvailableEvent)
                {
                    // Pick up current state
                    bool currentState = true;
                    foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
                    {
                        if(!ws.RunSettings.RunEnabled)
                        {
                            currentState = false;
                            break;
                        }
                    }

                    // Only notify if we are changing state e.g. the Revit context becomes available
                    if (!currentState)
                    {
                        OnRevitContextAvailable();
                    }
                }

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
                foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
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
                foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
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

            var nodes = ElementBinder.GetNodesFromElementIds(
                deleted,
                CurrentWorkspace,
                EngineController);
            foreach (var node in nodes)
            {
                node.OnNodeModified(forceExecute: true);
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
        
            var nodes = ElementBinder.GetNodesFromElementIds(
                updatedIds,
                CurrentWorkspace,
                EngineController);
            foreach (var node in nodes )
            {
            
                node.OnNodeModified(true);
            }
        }

        protected override void OpenFileImpl(OpenFileCommand command)
        {
            // MAGN-9824 changed file open to be executed right away instead of queue up.
            // This is to make sure DynamoRevit is consistent with DynamoStudio on file open.
            base.OpenFileImpl(command);
        }

        #endregion

    }
}

//Copyright © Autodesk, Inc. 2012. All rights reserved.
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace RevitServices.Elements
{
    public class RevitServicesUpdater
    {
        //TODO: To handle multiple documents, should store unique ids as opposed to ElementIds.

        private readonly ControlledApplication application;
        private readonly HashSet<IUpdater> registeredUpdaters;

        [Obsolete("Use ElementsUpdated event")]
        public event ElementUpdateDelegate ElementsAdded;
        [Obsolete("Use ElementsUpdated event")]
        public event ElementUpdateDelegateElementId ElementAddedForID;
        [Obsolete("Use ElementsUpdated event")]
        public event ElementUpdateDelegate ElementsModified;
        [Obsolete("Use ElementsUpdated event")]
        public event ElementDeleteDelegate ElementsDeleted;

        public event EventHandler<ElementUpdateEventArgs> ElementsUpdated;
        #region Event Invokers

        protected virtual void OnElementsAdded(ElementUpdateEventArgs e)
        {
            var handler = ElementsAdded;
            if (handler != null) handler(e.GetUniqueIds());
        }

        protected virtual void OnElementIdsAdded(ElementUpdateEventArgs e)
        {
            var handler = ElementAddedForID;
            if (handler != null) handler(e.RevitDocument, e.Elements);
            
            var updateHandler = ElementsUpdated;
            if (updateHandler != null) updateHandler(this, e);
        }


        protected virtual void OnElementsModified(ElementUpdateEventArgs e)
        {
            var handler = ElementsModified;
            if (handler != null) handler(e.GetUniqueIds());
            
            var updateHandler = ElementsUpdated;
            if (updateHandler != null) updateHandler(this, e);
        }

        protected virtual void OnElementsDeleted(ElementUpdateEventArgs e)
        {
            var handler = ElementsDeleted;
            if (handler != null) handler(e.RevitDocument, e.Elements);
            
            var updateHandler = ElementsUpdated;
            if (updateHandler != null) updateHandler(this, e);
        }

        #endregion

        #region Singleton

        /// <summary>
        ///     Initializes the static Instance of the RevitServicesUpdater.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="updaters"></param>
        public static void Initialize(ControlledApplication app, IEnumerable<IUpdater> updaters)
        {
            lock (mutex)
            {
                if (instance != null)
                    DisposeInstance();

                instance = new RevitServicesUpdater(app, updaters);
            }
        }

        /// <summary>
        ///     The static instance of the RevitServicesUpdater.
        /// </summary>
        public static RevitServicesUpdater Instance
        {
            get
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        throw new InvalidOperationException(
                            "RevitServicesUpdater has not been initialized.");
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        ///     Disposes the static intance of the RevitServicesUpdater. Can be re-initialized at a later point.
        /// </summary>
        public static void DisposeInstance()
        {
            lock (mutex)
            {
                if (instance == null)
                    return;

                instance.Dispose();
                instance = null;
            }
        }

        private static RevitServicesUpdater instance;
        private static readonly object mutex = new object();

        #endregion

        // constructor takes the AddInId for the add-in associated with this updater
        private RevitServicesUpdater(/*AddInId id, */ControlledApplication app, IEnumerable<IUpdater> updaters)
        {
            application = app;
            registeredUpdaters = new HashSet<IUpdater>(updaters);

            application.DocumentChanged += ApplicationDocumentChanged;

            foreach (var updater in registeredUpdaters)
            {
                ((ElementTypeSpecificUpdater)updater).Updated += RevitServicesUpdater_Updated;
            }
        }

        public void AddUpdater(IUpdater updater)
        {
            if (registeredUpdaters.Contains(updater)) 
                return;

            registeredUpdaters.Add(updater);
            ((ElementTypeSpecificUpdater)updater).Updated += RevitServicesUpdater_Updated;
        }

        public void RemoveUpdater(IUpdater updater)
        {
            if (!registeredUpdaters.Contains(updater)) 
                return;

            registeredUpdaters.Remove(updater);
            ((ElementTypeSpecificUpdater)updater).Updated -= RevitServicesUpdater_Updated;
        }

        /// <summary>
        /// Handler for the ElementTypeSpecificUpdater's Updated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void RevitServicesUpdater_Updated(object sender, UpdaterArgs args)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;

            // Are we loaded yet?
            if (doc == null)
                // No
                return;

            var addedIds = args.Added;
            var modified = args.Modified;
            var deleted = args.Deleted;
            ProcessUpdates(doc, modified, deleted, addedIds, Enumerable.Empty<string>());
        }

        private void Dispose()
        {
            application.DocumentChanged -= ApplicationDocumentChanged; 
            
            foreach (var updater in registeredUpdaters)
            {
                ((ElementTypeSpecificUpdater)updater).Updated -= RevitServicesUpdater_Updated;
            }
        }

        //TODO: remove once we are using unique ids
        /// <summary>
        /// Document that is being watched for changes.
        /// </summary>
        public Document DocumentToWatch
        {
            get { return DocumentManager.Instance.CurrentDBDocument; }
        }

        /// <summary>
        /// Forces all deletion callbacks to be called for given sequence of elements.
        /// </summary>
        /// <param name="doc">Document to perform the Rollback on.</param>
        /// <param name="deleted">Sequence of elements to have registered deletion callbacks invoked.</param>
        public void RollBack(Document doc, ICollection<ElementId> deleted)
        {
            var empty = new List<string>();
            ProcessUpdates(doc, Enumerable.Empty<ElementId>(), deleted, Enumerable.Empty<ElementId>(), empty);
        }

        private void ProcessUpdates(Document doc, IEnumerable<ElementId> modified, 
            IEnumerable<ElementId> deleted, IEnumerable<ElementId> addedIds, 
            IEnumerable<string> transactions)
        {
            OnElementsDeleted(new ElementUpdateEventArgs(doc, deleted.Distinct(), transactions, ElementUpdateEventArgs.UpdateType.Deleted));
            OnElementsModified(new ElementUpdateEventArgs(doc, modified.Distinct(), transactions, ElementUpdateEventArgs.UpdateType.Modified));
            OnElementsAdded(new ElementUpdateEventArgs(doc, addedIds.Distinct(), transactions, ElementUpdateEventArgs.UpdateType.Added));
            OnElementIdsAdded(new ElementUpdateEventArgs(doc, addedIds.Distinct(), transactions, ElementUpdateEventArgs.UpdateType.Added));
        }

        void ApplicationDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            var doc = args.GetDocument();
            var addedIds = args.GetAddedElementIds();
            var modified = args.GetModifiedElementIds();
            var deleted = args.GetDeletedElementIds();

            ProcessUpdates(doc, modified, deleted, addedIds, args.GetTransactionNames());
        }

        /// <summary>
        /// Clears all registered callbacks. This includes Modified and Deleted callbacks registered
        /// with RegisterChangeHook, and all delegates registered with the ElementsAdded event.
        /// </summary>
        public void UnRegisterAllChangeHooks()
        {
            ElementsAdded = null;
            ElementAddedForID = null;
            ElementsModified = null;
            ElementsDeleted = null;
            ElementsUpdated = null;
        }
    }

    /// <summary>
    /// Contains context data for an element update event.
    /// </summary>
    public class ElementUpdateEventArgs : EventArgs
    {
        public enum UpdateType
        {
            Added,
            Modified,
            Deleted,
        }
        /// <summary>
        /// Constructor for ElementUpdateEventArgs
        /// </summary>
        /// <param name="doc">Revit Document involved in the event.</param>
        /// <param name="elements">List of elements involved in the event.</param>
        /// <param name="transactions">Name of transactions involved in the event.</param>
        public ElementUpdateEventArgs(Document doc, IEnumerable<ElementId> elements, IEnumerable<string> transactions, UpdateType operation)
        {
            RevitDocument = doc;
            Elements = elements;
            Transactions = transactions;
            Operation = operation;
        }

        /// <summary>
        /// Returns unique ids for the elements involved in the event.
        /// </summary>
        /// <returns>List of unique id strings</returns>
        public IEnumerable<string> GetUniqueIds() { return Elements.Select(x => RevitDocument.GetElement(x).UniqueId); }

        /// <summary>
        /// List of elements involved in the event.
        /// </summary>
        public IEnumerable<ElementId> Elements { get; private set; }

        /// <summary>
        /// The Revit document involved in the event.
        /// </summary>
        public Document RevitDocument { get; private set; }

        /// <summary>
        /// The name of transactions involved in the event.
        /// </summary>
        public IEnumerable<string> Transactions { get; set; }

        /// <summary>
        /// Gets update operation type.
        /// </summary>
        public UpdateType Operation { get; private set; }
    }

    /// <summary>
    /// Callback for when Elements have been updated.
    /// </summary>
    /// <param name="updated">All modified elements that have been registered with this callback.</param>
    public delegate void ElementUpdateDelegate(IEnumerable<string> updated);


    /// <summary>
    /// Callback for when Elements have been updated.
    /// Recoemnt using the UUID version instead
    /// </summary>
    /// <param name="updated">All modified elements that have been registered with this callback.</param>
    public delegate void ElementUpdateDelegateElementId(Document document, IEnumerable<ElementId> deleted);


    /// <summary>
    ///     Callback for when Elements have been deleted.
    /// </summary>
    /// <param name="document">Document from which deleted elements originated.</param>
    /// <param name="deleted">The deleted ElementIds.</param>
    public delegate void ElementDeleteDelegate(Document document, IEnumerable<ElementId> deleted);
}

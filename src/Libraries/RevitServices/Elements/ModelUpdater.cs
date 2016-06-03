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

        private readonly HashSet<IUpdater> registeredUpdaters;

        public event ElementUpdateDelegate ElementsAdded;
        public event ElementUpdateDelegateElementId ElementAddedForID;
        public event ElementUpdateDelegate ElementsModified;
        public event ElementDeleteDelegate ElementsDeleted;

        #region Event Invokers

        protected virtual void OnElementsAdded(IEnumerable<string> updated)
        {
            var handler = ElementsAdded;
            if (handler != null) handler(updated);
        }

        protected virtual void OnElementsAdded(Document doc, IEnumerable<ElementId> updated)
        {
            var handler = ElementAddedForID;
            if (handler != null) handler(doc, updated);
        }


        protected virtual void OnElementsModified(IEnumerable<string> updated)
        {
            var handler = ElementsModified;
            if (handler != null) handler(updated);
        }

        protected virtual void OnElementsDeleted(Document document, IEnumerable<ElementId> deleted)
        {
            var handler = ElementsDeleted;
            if (handler != null) handler(document, deleted);
        }

        #endregion

        #region Singleton

        /// <summary>
        ///     Initializes the static Instance of the RevitServicesUpdater.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="updaters"></param>
        public static void Initialize(IEnumerable<IUpdater> updaters)
        {
            lock (mutex)
            {
                if (instance != null)
                    DisposeInstance();

                instance = new RevitServicesUpdater(updaters);
            }
        }

        public static bool IsInitialized
        {
            get
            {
                lock (mutex)
                {
                    return instance != null;
                }
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
        private RevitServicesUpdater(/*AddInId id, */IEnumerable<IUpdater> updaters)
        {
            registeredUpdaters = new HashSet<IUpdater>(updaters);

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

            var added = args.Added.Select(x => doc.GetElement(x).UniqueId);
            var addedIds = args.Added;
            var modified = args.Modified.Select(x => doc.GetElement(x).UniqueId).ToList();
            var deleted = args.Deleted;
            ProcessUpdates(doc, modified, deleted, added, addedIds);
        }

        private void Dispose()
        {            
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
            ProcessUpdates(doc, empty, deleted, empty, new List<ElementId>());
        }

        private void ProcessUpdates(Document doc, IEnumerable<string> modified, 
            IEnumerable<ElementId> deleted, IEnumerable<string> added, 
            IEnumerable<ElementId> addedIds )
        {
            OnElementsDeleted(doc, deleted.Distinct());
            OnElementsModified(modified.Distinct());
            OnElementsAdded(added.Distinct());
            OnElementsAdded(doc, addedIds);
        }

        public void ApplicationDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            //skip processing element modification for nodes modified due to Dynamo script transaction.
            if (args.GetTransactionNames().All(x => string.Equals(x, TransactionWrapper.TransactionName)))
                return;

            var doc = args.GetDocument();
            var added = args.GetAddedElementIds().Select(x => doc.GetElement(x).UniqueId);
            var addedIds = args.GetAddedElementIds();
            var modified = args.GetModifiedElementIds().Select(x => doc.GetElement(x).UniqueId).ToList();
            var deleted = args.GetDeletedElementIds();

            ProcessUpdates(doc, modified, deleted, added, addedIds);
        }

        /// <summary>
        /// Clears all registered callbacks. This includes Modified and Deleted callbacks registered
        /// with RegisterChangeHook, and all delegates registered with the ElementsAdded event.
        /// </summary>
        public void UnRegisterAllChangeHooks()
        {
            ElementsAdded = null;
            ElementsModified = null;
            ElementsDeleted = null;
        }
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

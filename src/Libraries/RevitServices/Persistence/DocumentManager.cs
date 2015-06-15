using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitServices.Elements;
using RevitServices.Threading;
using RevitServices.Transactions;

namespace RevitServices.Persistence
{

    /// <summary>
    /// Singleton class to manage Revit document resources
    /// </summary>
    public class DocumentManager
    {
        public static event Action<string> OnLogError;

        internal static void LogError(string obj)
        {
            var handler = OnLogError;
            if (handler != null)
                handler(obj);
        }

        internal static void LogError(Exception exception)
        {
            var handler = OnLogError;
            if (handler != null)
                handler(exception.Message);
        }

        private static DocumentManager instance;
        private static readonly Object mutex = new Object();

        public static DocumentManager Instance
        {
            get
            {
                lock (mutex)
                {
                    return instance ?? (instance = new DocumentManager());
                }
            }
        }

        private DocumentManager() { }

        /// <summary>
        /// Determine if Element exists in the current document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ElementExistsInDocument(ElementUUID id)
        {
            Element e;
            return CurrentDBDocument.TryGetElement(id.UUID, out e);
        }

        /// <summary>
        /// Delete an element from the current document given the ElementUUID
        /// </summary>
        /// <param name="element">The UUID of the element to delete</param>
        public void DeleteElement(ElementUUID element)
        {
            ElementId id = ElementBinder.GetIdForUUID(CurrentDBDocument, element);

            if (null != id)
            {
                TransactionManager.Instance.EnsureInTransaction(CurrentDBDocument);

                CurrentDBDocument.Delete(id);

                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Obtain all elements of a given type from the current document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ElementsOfType<T>() where T : Element
        {
            var fec = new FilteredElementCollector(CurrentDBDocument);
            return fec.OfClass(typeof(T)).Cast<T>();
        }

        public IEnumerable<Element> ElementsOfCategory(BuiltInCategory category) 
        {
            var fec = new FilteredElementCollector(CurrentDBDocument);
            return fec.OfCategory(category);
        }

        /// <summary>
        /// Handle document activation, closure. See ActiveDocumentHashCode 
        /// for more details.
        /// </summary>
        /// <param name="revitView">The Revit.DB.View object that is being 
        /// activated. This parameter will be null if the currently active 
        /// document is closed. If there is another document when the current 
        /// document is closed, the next document will be activated again 
        /// after active document is invalidated here.</param>
        /// 
        public void HandleDocumentActivation(View revitView)
        {
            ActiveDocumentHashCode = 0;
            if (revitView != null && (revitView.Document != null))
                ActiveDocumentHashCode = revitView.Document.GetHashCode();
        }

        /// <summary>
        /// Provides the currently active DB document.
        /// This is based on the CurrentUIDocument
        /// </summary>
        public Document CurrentDBDocument {
            get
            {
                var c = CurrentUIDocument;
                return c == null ? null : c.Document;
            }
        }

        /// <summary>
        /// This property represents the hash code for the current active Revit
        /// Document object. If there is no active document, this value will be 
        /// set to zero. This property is made an 'int' because it is primarily
        /// meant to indicate the active document for comparison purposes, no 
        /// Document specific operations should be allowed on it. This property 
        /// is updated when a document switch or a document closure happens. The
        /// reason this property is needed because the following property cannot
        /// reliably indicate the current active document:
        /// 
        ///     DocumentManager.CurrentUIApplication.ActiveUIDocument.Document
        /// 
        /// </summary>
        public int ActiveDocumentHashCode { get; private set; }

        /// <summary>
        /// Provides the currently active UI document.
        /// This is the document to which Dynamo is bound.
        /// </summary>
        public UIDocument CurrentUIDocument {get; set; }

        /// <summary>
        /// Provides the current UIApplication
        /// </summary>
        public UIApplication CurrentUIApplication { get; set; }

        /// <summary>
        /// Trigger a document regeneration in the idle context or without
        /// depending on the state of the transaction manager.
        /// </summary>
        public static void Regenerate()
        {
            if (TransactionManager.Instance.DoAssertInIdleThread)
            {
                TransactionManager.Instance.EnsureInTransaction(
                    DocumentManager.Instance.CurrentDBDocument);
                Instance.CurrentDBDocument.Regenerate();
            }
            else
            {
                Instance.CurrentDBDocument.Regenerate();
            }
        }
    }
}

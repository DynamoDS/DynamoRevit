using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit TextNoteType
    /// </summary>
    public class TextNoteType : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.TextNoteType InternalTextNoteType
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalTextNoteType; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private TextNoteType(Autodesk.Revit.DB.TextNoteType type)
        {
            SafeInit(() => InitTextNoteType(type));
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a ModelTextType element
        /// </summary>
        /// <param name="type"></param>
        private void InitTextNoteType(Autodesk.Revit.DB.TextNoteType type)
        {
            InternalSetTextNoteType(type);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="type"></param>
        private void InternalSetTextNoteType(Autodesk.Revit.DB.TextNoteType type)
        {
            this.InternalTextNoteType = type;
            this.InternalElementId = type.Id;
            this.InternalUniqueId = type.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a ModelTextType from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TextNoteType ByName(string name)
        {
            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.TextNoteType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(String.Format(Properties.Resources.ModelTextTypeNotFound, name));
            }

            return new TextNoteType(type)
            {
                IsRevitOwned = true
            };
        }


        /// <summary>
        /// Return a default TextNoteType
        /// </summary>
        /// <returns></returns>
        public static TextNoteType Default()
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(Autodesk.Revit.DB.TextNoteType));
            return FromExisting((Autodesk.Revit.DB.TextNoteType)collector.FirstOrDefault(), true);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from an existing Revit element
        /// </summary>
        /// <param name="modelTextType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static TextNoteType FromExisting(Autodesk.Revit.DB.TextNoteType type, bool isRevitOwned)
        {
            return new TextNoteType(type)
            {
                IsRevitOwned = isRevitOwned
            };
        }


        #endregion

    }
}

using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit TextNoteType
    /// </summary>
    public class TextNoteType : ElementType
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.TextNoteType InternalTextNoteType => InternalElementType as Autodesk.Revit.DB.TextNoteType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="textNoteType"></param>
        private TextNoteType(Autodesk.Revit.DB.TextNoteType textNoteType) : base(textNoteType)
        {
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a ModelTextType from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new TextNoteType ByName(string name)
        {
            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.TextNoteType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(String.Format(Properties.Resources.TypeNotFound, name));
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
        /// <param name="type"></param>
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

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using DSCore.IO;
using DynamoServices;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit ElementType
    /// </summary>
    public class ElementType : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal reference to the ElementType.
        /// </summary>
        internal Autodesk.Revit.DB.ElementType InternalElementType
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element.
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalElementType; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element.
        /// </summary>
        /// <param name="elementType"></param>
        private protected ElementType(Autodesk.Revit.DB.ElementType elementType)
        {
            SafeInit(() => InitElementType(elementType));
        }

        /// <summary>
        /// Initialize a ElementType element
        /// and sets the ElementType property, element id, and unique id.
        /// </summary>
        /// <param name="elementType"></param>
        private void InitElementType(Autodesk.Revit.DB.ElementType elementType)
        {
            this.InternalElementType = elementType;
            this.InternalElementId = elementType.Id;
            this.InternalUniqueId = elementType.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the ElementType.
        /// </summary>
        public new string Name
        {
            get { return InternalElementType.Name; }
        }

        /// <summary>
        /// The FamilyName of the ElementType.
        /// </summary>
        public  string FamilyName
        {
            get { return InternalElementType.FamilyName; }
        }

        /// <summary>
        /// Determine if this ElementType can be deleted.
        /// </summary>
        public bool CanBeDeleted
        {
            get { return InternalElementType.CanBeDeleted; }
        }

        /// <summary>
        /// Determine if this ElementType can be copied.
        /// </summary>
        public bool CanBeCopied
        {
            get { return InternalElementType.CanBeCopied; }
        }

        /// <summary>
        /// Determine if this ElementType can be renamed.
        /// </summary>
        public bool CanBeRenamed
        {
            get { return InternalElementType.CanBeRenamed; }
        }

        #endregion

        #region Public constructors

        /// <summary>
        /// Duplicates an existing element type and assigns it a new name.
        /// </summary>
        /// <param name="name">The new name of the element type.</param>
        /// <returns>The duplicated element type.</returns>
        public ElementType Duplicate(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            TransactionManager.Instance.EnsureInTransaction(Document);
            ElementType newElementType = FromExisting(this.InternalElementType.Duplicate(name), true);
            TransactionManager.Instance.TransactionTaskDone();
            return newElementType;  
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Returns the type Element with the given name.
        /// </summary>
        /// <param name="name">Name of the type</param>
        /// <returns>Type Element</returns>
        public static ElementType ByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var elementType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.ElementType>()
                .FirstOrDefault(x => x.Name == name);

            if (elementType == null)
                throw new KeyNotFoundException(Properties.Resources.ElementTypeNameNotFound);
            return FromExisting(elementType, true);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a ElementType from a user selected Element.
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ElementType FromExisting(Autodesk.Revit.DB.ElementType elementType, bool isRevitOwned)
        {
            return new ElementType(elementType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get the preview image of an element. This image is similar to what is seen in
        /// the Revit UI when selecting the type of an element.
        /// </summary>
        /// <param name="size">The width and height of the preview image in pixels.</param>
        /// <returns>The preview image. null if there is no preview image.</returns>
        public Bitmap GetPreviewImage(int size = 500)
        {
            System.Drawing.Size imageSize = new System.Drawing.Size(size, size);
            Bitmap bitmapImage = this.InternalElementType.GetPreviewImage(imageSize);
            return bitmapImage;
        }

        #endregion
    }
}

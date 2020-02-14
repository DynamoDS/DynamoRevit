﻿using System;
using System.Linq;
using Autodesk.Revit.DB;
using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyType, the Revit API refers to this as a FamilySymbol
    /// </summary>
    [RegisterForTrace]
    public class FamilyType: ElementType
    {

        #region Internal Properties

        /// <summary>
        /// Internal wrapper property
        /// </summary>
        internal Autodesk.Revit.DB.FamilySymbol InternalFamilySymbol => InternalElementType as Autodesk.Revit.DB.FamilySymbol;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for building a DSFamilySymbol
        /// </summary>
        /// <param name="symbol"></param>
        private FamilyType(Autodesk.Revit.DB.FamilySymbol symbol):base(symbol)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get the name of this Family Type
        /// </summary>
        public new string Name
        {
            get
            {
                return InternalFamilySymbol.Name;
            }
        }

        /// <summary>
        /// Get the parent family of this FamilyType
        /// </summary>
        public Family Family
        {
            get
            {
                return Family.FromExisting(this.InternalFamilySymbol.Family, true);
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a FamilyType given its parent Family and the FamilyType's name.
        /// </summary>
        /// <param name="family">The FamilyTypes's parent Family</param>
        /// <param name="name">The name of the FamilyType</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static FamilyType ByFamilyAndName(Family family, string name)
        {
            if (family == null)
            {
                throw new ArgumentNullException("family");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            // obtain the family symbol with the provided name
            var symbol =
                family.InternalFamily.GetFamilySymbolIds().Select(x=>Document.GetElement(x)).
                Cast<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == name);

            if (symbol == null)
            {
               throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound2, name));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Select a FamilyType give it's family name and type name.
        /// </summary>
        /// <param name="familyName">The FamilyType's parent Family name.</param>
        /// <param name="typeName">The name of the FamilyType.</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static FamilyType ByFamilyNameAndTypeName(string familyName, string typeName)
        {
            if (familyName == null)
            {
                throw new ArgumentNullException("familyName");
            }

            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            //find the family
            var collector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            collector.OfClass(typeof (Autodesk.Revit.DB.Family));
            var family = (Autodesk.Revit.DB.Family)collector.ToElements().FirstOrDefault(x => x.Name == familyName);

            if (family == null)
            {
                throw new Exception(string.Format(Properties.Resources.FamilyNotFound, familyName));
            }

            // obtain the family symbol with the provided name
            var symbol =
                family.GetFamilySymbolIds().Select(x => DocumentManager.Instance.CurrentDBDocument.GetElement(x)).
                OfType<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == typeName);

            if (symbol == null)
            {
                throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound2, typeName));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Select a FamilyType given it's name.  This method will return the first FamilyType it finds if there are
        /// two or more FamilyTypes with the same name.
        /// </summary>
        /// <param name="name">The name of the FamilyType</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static new FamilyType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            // look up the loaded family
            var symbol = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.FamilySymbol>()
                .FirstOrDefault(x => x.Name == name);

            if (symbol == null)
            {
                throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound3, name));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Create new Family Type from a solid geometry.
        /// This method exports the geometry to SAT and imports it into
        /// a new family document.
        /// </summary>
        /// <param name="solidGeometry"></param>
        /// <param name="name">Name fo the Family Type</param>
        /// <param name="category">Family Type Category</param>
        /// <param name="templatePath">Family Template to use for creation</param>
        /// <param name="material">Material to apply to the solids</param>
        /// <param name="subcategory">Subcategory for the Family Type (optional)</param>
        /// <returns>Family Type</returns>
        public static FamilyType ByGeometry(Autodesk.DesignScript.Geometry.Solid solidGeometry, string name, Category category, string templatePath, Material material, string subcategory = "")
        {
            var symbol = solidGeometry.ToRevitFamilyType(name, category, templatePath, material, false, subcategory);

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Create a Void Family Type from a solid geometry.
        /// This method exports the solid to SAT and imports it into
        /// a new family document.
        /// </summary>
        /// <param name="solidGeometry"></param>
        /// <param name="name">Name to apply to the Family Type</param>
        /// <param name="category">Category to apply</param>
        /// <param name="templatePath">Template file to use for creation</param>
        /// <returns>Void Family Type</returns>
        public static FamilyType VoidByGeometry(Autodesk.DesignScript.Geometry.Solid solidGeometry, string name, Category category, string templatePath)
        {
            var symbol = solidGeometry.ToRevitFamilyType(name, category, templatePath, null, true, string.Empty);

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }


        #endregion

        #region Internal static constructors

        /// <summary>
        /// Obtain a FamilyType by selection. 
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FamilyType FromExisting(Autodesk.Revit.DB.FamilySymbol familyType, bool isRevitOwned)
        {
            return new FamilyType(familyType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region ToString override

        public override string ToString()
        {
            return String.Format("Family Type: {0}, Family: {1}", InternalFamilySymbol.Name,
                InternalFamilySymbol.Family.Name);
        }

        #endregion

    }

}

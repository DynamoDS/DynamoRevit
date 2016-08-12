using System;
using System.Linq;
using Autodesk.Revit.DB;
using DynamoServices;
using RevitServices.Persistence;
using Revit.GeometryConversion;
using RevitServices.Transactions;
using DynamoUnits;
using Revit.Elements.InternalUtilities;

using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyType, the Revit API refers to this as a FamilySymbol
    /// </summary>
    [RegisterForTrace]
    public class FamilyType: Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal wrapper property
        /// </summary>
        internal Autodesk.Revit.DB.FamilySymbol InternalFamilySymbol
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalFamilySymbol; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for building a DSFamilySymbol
        /// </summary>
        /// <param name="symbol"></param>
        private FamilyType(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            SafeInit(() => InitFamilySymbol(symbol));
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a FamilySymbol element
        /// </summary>
        /// <param name="symbol"></param>
        private void InitFamilySymbol(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            InternalSetFamilySymbol(symbol);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal model of the family symbol along with its ElementId and UniqueId
        /// </summary>
        /// <param name="symbol"></param>
        private void InternalSetFamilySymbol(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            this.InternalFamilySymbol = symbol;
            this.InternalElementId = symbol.Id;
            this.InternalUniqueId = symbol.UniqueId;
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
        public static FamilyType ByName(string name)
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
        /// Get Solid from Element helper method
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Solid GetSolidFromElement(Autodesk.Revit.DB.Element element)
        {
            GeometryElement geo = element.get_Geometry(new Options() { ComputeReferences = true });
            var enumerator = geo.GetEnumerator();
            enumerator.MoveNext();
            GeometryInstance i = (GeometryInstance)enumerator.Current;
            var geom2 = i.GetInstanceGeometry();
            var enumarator2 = geom2.GetEnumerator();
            enumarator2.MoveNext();
            return (Solid)enumarator2.Current;
        }

        /// <summary>
        /// Set Family Parameter Helper method
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        private static void SetFamilyParameter(Autodesk.Revit.DB.Element element, BuiltInParameter parameter, object value)
        {
            Autodesk.Revit.DB.Parameter p = element.get_Parameter(parameter);
            if (p != null && !p.IsReadOnly)
            {
                if (value.GetType() == typeof(ElementId)) { p.Set((ElementId)value); }
                if (value.GetType() == typeof(double)) { p.Set((double)value); }
                if (value.GetType() == typeof(int)) { p.Set((int)value); }
                if (value.GetType() == typeof(string)) { p.Set((string)value); }
            }
        }
        /// <summary>
        /// Create new Family Instance from Geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="templatePath"></param>
        /// <param name="isVoid"></param>
        /// <param name="subcategory"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static FamilyType ByGeometry(Autodesk.DesignScript.Geometry.Geometry geometry, string name, Category category, string templatePath, bool isVoid = false, string subcategory = "", string material = "")
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.ForceCloseTransaction();

            // create a temp sat file
            string tempFile = System.IO.Path.GetTempFileName() + ".sat";

            // create a temp family file
            string tempDir = System.IO.Path.GetTempPath();
            string tempFamilyFile = tempDir + "\\" + name + ".rfa";

            // Get the conversion factor based on the display units
            DisplayUnitType units = document.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            double factor = UnitUtils.ConvertToInternalUnits(1, units);

            // scale the incoming geometry
            geometry = geometry.Scale(factor);

            // get a displacement vector
            Vector vector = Vector.ByTwoPoints(Autodesk.DesignScript.Geometry.BoundingBox.ByGeometry(geometry).MinPoint, Autodesk.DesignScript.Geometry.Point.Origin());

            // translate the geometry to origin
            geometry = geometry.Translate(vector);

            // export geometry to SAT
            geometry.ExportToSAT(tempFile);

            // create a new family document using the supplied template
            Autodesk.Revit.DB.Document familyDocument = document.Application.NewFamilyDocument(templatePath);

            // Get the families 3d view
            var collector = new Autodesk.Revit.DB.FilteredElementCollector(familyDocument).OfClass(typeof(Autodesk.Revit.DB.View));
            Autodesk.Revit.DB.View view = null;
            foreach (Autodesk.Revit.DB.View v in collector.ToElements())
            {
                if (!v.IsTemplate && v.ViewType == ViewType.ThreeD)
                {
                    view = v;
                }
            }

            // Import the sat file to origin in feet
            TransactionManager.Instance.EnsureInTransaction(familyDocument);
            ElementId importedElementId = familyDocument.Import(tempFile, new SATImportOptions() { Placement = ImportPlacement.Origin, Unit = ImportUnit.Foot }, view);

            // get the solid element from the imported sat file
            Solid solid = GetSolidFromElement(familyDocument.GetElement(importedElementId));

            // delete imported sat
            familyDocument.Delete(importedElementId);
            System.IO.File.Delete(tempFile);

            // Set the families category
            familyDocument.OwnerFamily.FamilyCategory = familyDocument.Settings.Categories.get_Item(category.Name);

            // create a free form element from the solid geometry
            FreeFormElement freeFormElement = FreeFormElement.Create(familyDocument, solid);

            // if the geometry should be void set parameters accordingly
            if (isVoid)
            {
                SetFamilyParameter(freeFormElement, BuiltInParameter.ELEMENT_IS_CUTTING, 1);
                SetFamilyParameter(freeFormElement, BuiltInParameter.FAMILY_ALLOW_CUT_WITH_VOIDS, 1);
            }
            else
            {
                // Apply material if supplied
                if (material != string.Empty)
                {
                    Autodesk.Revit.DB.FilteredElementCollector materialCollector = new Autodesk.Revit.DB.FilteredElementCollector(familyDocument).OfClass(typeof(Autodesk.Revit.DB.Material));
                    foreach (Autodesk.Revit.DB.Material mat in materialCollector)
                    {
                        if (mat.Name == material)
                        {
                            SetFamilyParameter(freeFormElement, BuiltInParameter.MATERIAL_ID_PARAM, mat.Id);
                        }
                    }
                }

                // Apply Subcategory if supplied
                if (subcategory != string.Empty)
                {
                    var current_fam_cat = familyDocument.OwnerFamily.FamilyCategory;
                    var new_subcat = familyDocument.Settings.Categories.NewSubcategory(current_fam_cat, subcategory);
                    freeFormElement.Subcategory = new_subcat;
                }
            }

            TransactionManager.Instance.ForceCloseTransaction();

            // Save Family document and load it into the project
            familyDocument.SaveAs(tempFamilyFile, new SaveAsOptions() { OverwriteExistingFile = true });
            var family = familyDocument.LoadFamily(document, new FamOpt1());

            // close and delete family
            familyDocument.Close(false);
            System.IO.File.Delete(tempFamilyFile);

            // get imported family symbol
            var symbols = family.GetFamilySymbolIds().GetEnumerator();
            symbols.MoveNext();
            FamilySymbol symbol1 = (FamilySymbol)document.GetElement(symbols.Current);

            // activate symbol
            TransactionManager.Instance.EnsureInTransaction(document);
            if (!symbol1.IsActive) symbol1.Activate();

            return new FamilyType(symbol1)
            {
                IsRevitOwned = true
            };

            // place instance in  correct location
            //Autodesk.Revit.DB.FamilyInstance instance = document.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbol1, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            //ElementTransformUtils.MoveElement(document, instance.Id, vector.Reverse().ToXyz());

            //TransactionManager.Instance.ForceCloseTransaction();
            //return new FamilyInstance(instance);
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

    public class FamOpt1 : IFamilyLoadOptions
    {
        public FamOpt1() { }

        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Autodesk.Revit.DB.Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Project;
            overwriteParameterValues = true;
            return true;
        }

    }
}

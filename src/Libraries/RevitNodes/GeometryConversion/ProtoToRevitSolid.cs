using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Materials;
using Autodesk.DesignScript.Geometry;
using Dynamo.Visualization;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.GeometryConversion
{
    [IsVisibleInDynamoLibrary(false)]
    [SupressImportIntoVM]
    public static class ProtoToRevitSolid
    {

        /// <summary>
        /// Get Solids from Element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static IList<Autodesk.Revit.DB.Solid> GetSolidsFromElement(Autodesk.Revit.DB.Element element)
        {
            GeometryElement geo = element.get_Geometry(new Options() { ComputeReferences = true });

            List<Autodesk.Revit.DB.Solid> solids = new List<Autodesk.Revit.DB.Solid>();

            foreach (object obj in geo)
            {
                GeometryInstance geoinstance = obj as GeometryInstance;
                if (geoinstance != null)
                {
                    foreach (object solidobj in geoinstance.GetInstanceGeometry())
                    {
                        if (solidobj.GetType() == typeof(Autodesk.Revit.DB.Solid))
                        {
                            Autodesk.Revit.DB.Solid solid = solidobj as Autodesk.Revit.DB.Solid;
                            solids.Add(solid);
                        }
                    }
                }
            }

            return solids;
        }

        /// <summary>
        /// Apply Material to Solid
        /// </summary>
        /// <param name="familyDocument"></param>
        /// <param name="material"></param>
        /// <param name="element"></param>
        private static void ApplyMaterialToFreeForm(
            Autodesk.Revit.DB.Document familyDocument, 
            Revit.Elements.Material material,
            Autodesk.Revit.DB.FreeFormElement freeform)
        {
            var materialCollector = new Autodesk.Revit.DB.FilteredElementCollector(familyDocument)
                .OfClass(typeof(Autodesk.Revit.DB.Material));

            foreach (Autodesk.Revit.DB.Material mat in materialCollector)
            {
                if (mat.Name == material.Name)
                {
                    var materialParam = freeform.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
                    if (materialParam != null && !materialParam.IsReadOnly)
                    {
                        Revit.Elements.InternalUtilities.ElementUtils.SetParameterValue(materialParam, material);
                    }
                }
            }
        }

        /// <summary>
        /// Apply SubCategory to Solid
        /// </summary>
        /// <param name="familyDocument"></param>
        /// <param name="subcategory"></param>
        /// <param name="freeform"></param>
        private static void ApplySubCategoryToFreeForm(
            Autodesk.Revit.DB.Document familyDocument,
            string subcategory,
            Autodesk.Revit.DB.FreeFormElement freeform)
        {
            var currentFamilyCategory = familyDocument.OwnerFamily.FamilyCategory;
            var newSubCategory = familyDocument.Settings.Categories.NewSubcategory(currentFamilyCategory, subcategory);
            freeform.Subcategory = newSubCategory;
        }

        /// <summary>
        /// Apply Cutting Settings to Void
        /// </summary>
        /// <param name="freeform"></param>
        private static void ApplyVoidSettingsToFreeForm(Autodesk.Revit.DB.FreeFormElement freeform)
        {
            var elementIsCuttingParameter = freeform.get_Parameter(BuiltInParameter.ELEMENT_IS_CUTTING);
            if (elementIsCuttingParameter != null && !elementIsCuttingParameter.IsReadOnly)
            {
                Revit.Elements.InternalUtilities.ElementUtils.SetParameterValue(elementIsCuttingParameter, 1);
            }

            var cutWithVoidsParameter = freeform.get_Parameter(BuiltInParameter.FAMILY_ALLOW_CUT_WITH_VOIDS);
            if (cutWithVoidsParameter != null && !cutWithVoidsParameter.IsReadOnly)
            {
                Revit.Elements.InternalUtilities.ElementUtils.SetParameterValue(cutWithVoidsParameter, 1);
            }
        }

        /// <summary>
        /// Convert a DS Solid to a Revit FamilySymbol containing 
        /// Revit FreeForms via SAT Export/Import
        /// </summary>
        /// <param name="solidGeometry"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="templatePath">Revit Template to use for Family Creation</param>
        /// <param name="material">Can be null for Voids</param>
        /// <param name="isVoid">Create Void</param>
        /// <param name="subcategory">Can be string.Empty for Voids</param>
        /// <returns></returns>
        public static Autodesk.Revit.DB.FamilySymbol ToRevitFamilyType(
            this Autodesk.DesignScript.Geometry.Solid solidGeometry, 
            string name,
            Revit.Elements.Category category, 
            string templatePath,
            Revit.Elements.Material material, 
            bool isVoid, 
            string subcategory = "")
        {
            // Keep the current document and close the open transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.ForceCloseTransaction();

            // create a temp sat file
            string tempFile = System.IO.Path.GetTempFileName() + ".sat";

            // create a temp family file
            string tempDir = System.IO.Path.GetTempPath();
            string tempFamilyFile = tempDir + "\\" + name + ".rfa";

            // scale the incoming geometry
            UnitConverter.ConvertToHostUnits<Autodesk.DesignScript.Geometry.Solid>(ref solidGeometry);

            // get a displacement vector
            Vector vector = Vector.ByTwoPoints(Autodesk.DesignScript.Geometry.BoundingBox.ByGeometry(solidGeometry).MinPoint, Autodesk.DesignScript.Geometry.Point.Origin());

            // translate the geometry to origin
            solidGeometry = solidGeometry.Translate(vector) as Autodesk.DesignScript.Geometry.Solid;

            // export geometry to SAT
            solidGeometry.ExportToSAT(tempFile);
            solidGeometry.Dispose();

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

            // Open a Transaction with the FamilyDocument
            TransactionManager.Instance.EnsureInTransaction(familyDocument);

            // Import the sat file to origin in feet
            ElementId importedElementId = familyDocument.Import(tempFile, new SATImportOptions() { Placement = ImportPlacement.Origin, Unit = ImportUnit.Foot }, view);

            // get the solid element from the imported sat file
            var solids = GetSolidsFromElement(familyDocument.GetElement(importedElementId));

            // delete imported sat
            familyDocument.Delete(importedElementId);
            System.IO.File.Delete(tempFile);

            // Set the families category
            familyDocument.OwnerFamily.FamilyCategory = familyDocument.Settings.Categories.get_Item(category.Name);

            foreach (var solid in solids)
            {
                // Create Freeform Element
                var freeform = FreeFormElement.Create(familyDocument, solid);

                // if the geometry should be void set parameters accordingly
                if (isVoid)
                {
                    ApplyVoidSettingsToFreeForm(freeform);
                }
                else
                {
                    // Apply material if supplied
                    ApplyMaterialToFreeForm(familyDocument, material, freeform);

                    // Apply Subcategory if supplied
                    if (subcategory != string.Empty)
                    {
                        ApplySubCategoryToFreeForm(familyDocument, subcategory, freeform);
                    }
                }
            }
                
            // Close the FamilyDocument Transaction
            TransactionManager.Instance.ForceCloseTransaction();

            // Save Family document and load it into the project
            familyDocument.SaveAs(tempFamilyFile, new SaveAsOptions() { OverwriteExistingFile = true });
            var family = familyDocument.LoadFamily(document, new FamilyImportOptions());

            // close and delete family
            familyDocument.Close(false);
            System.IO.File.Delete(tempFamilyFile);

            // get first imported family symbol
            var symbols = family.GetFamilySymbolIds();

            // Restore the Project Document Transaction
            TransactionManager.Instance.EnsureInTransaction(document);

            if (symbols.Count > 0)
            {
                FamilySymbol symbol = (FamilySymbol)document.GetElement(symbols.First());

                // activate symbol
                if (!symbol.IsActive) symbol.Activate();

                return symbol;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Family Import Options
    /// </summary>
    public class FamilyImportOptions : IFamilyLoadOptions
    {
        public FamilyImportOptions() { }

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

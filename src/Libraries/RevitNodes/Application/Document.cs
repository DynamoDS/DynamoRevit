
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Elements;
using Revit.GeometryConversion;
using System;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DynamoUnits;
using View = Revit.Elements.Views.View;

namespace Revit.Application
{
    /// <summary>
    /// A Revit Document
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Internal reference to the Document
        /// </summary>
        internal Autodesk.Revit.DB.Document InternalDocument { get; private set; }

        internal Document(Autodesk.Revit.DB.Document currentDBDocument)
        {
            InternalDocument = currentDBDocument;  
        }

        /// <summary>
        /// Get the active view for the document
        /// </summary>
        public View ActiveView
        {
            get
            {
                return (View)InternalDocument.ActiveView.ToDSType(true);
            }
        }

        /// <summary>
        /// Is the Document a Family?
        /// </summary>
        public bool IsFamilyDocument
        {
            get
            {
                return InternalDocument.IsFamilyDocument;
            }
        }

        /// <summary>
        /// The full path of the Document.
        /// </summary>
        public string FilePath
        {
            get { return InternalDocument.PathName ?? string.Empty; }
        }

        /// <summary>
        /// Get the current document
        /// </summary>
        /// <returns></returns>
        public static Document Current
        {
            get { return new Document(DocumentManager.Instance.CurrentDBDocument); }
        }

        private List<PerformanceAdviserRuleId> performanceAdviserRuleIds;
        private List<PerformanceAdviserRuleId> PerformanceAdviserRuleIds
        {
            get
            {
                if (performanceAdviserRuleIds == null)
                {
                    performanceAdviserRuleIds = GetPerfromanceAdviserRuleIdForPurge();
                }
                return performanceAdviserRuleIds;
            }
        }

        /// <summary>
        /// Gets the worksharing path of the current document
        /// </summary>
        public string WorksharingPath
        {
            get
            {
                ModelPath modelPath = WorksharingModelPath;
                if (modelPath == null)
                    throw new NullReferenceException(Properties.Resources.DocumentNotWorkshared);
                return ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath); 
            }
        }

        /// <summary>
        /// Whether the Worksharing path represents a path on an Autodesk server such as BIM360.
        /// </summary>
        public bool IsCloudPath
        {
            get{ return WorksharingModelPath.CloudPath; }
        }

        internal ModelPath WorksharingModelPath
        {
            get { return this.InternalDocument.GetWorksharingCentralModelPath(); }
        }

        /// <summary>
        /// Purge unused Elements from the model.
        /// </summary>
        /// <param name="fullPurge">Similar to clicking the purge button multiple times in Revit, the node will repeatedly purge until there is nothing left to remove.</param>
        /// <returns>Purged element ids</returns>
        public List<long> PurgeUnused(bool fullPurge = false)
        {
            List<ElementId> purgedElementIds = new List<ElementId>();
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);

            purgedElementIds.AddRange(PurgeElements(fullPurge));
            purgedElementIds.AddRange(PurgeMaterials());

            TransactionManager.Instance.TransactionTaskDone();

            if (purgedElementIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoElementsToPurge);

            return purgedElementIds.Select(x => x.Value).ToList();
        }

        private List<ElementId> PurgeMaterials()
        {
            List<Autodesk.Revit.DB.ElementId> materials = new FilteredElementCollector(this.InternalDocument)
                .OfClass(typeof(Autodesk.Revit.DB.Material))
                .ToElementIds()
                .ToList();

            if (materials == null)
                return new List<ElementId>();

            List<Autodesk.Revit.DB.Element> elementTypes = new FilteredElementCollector(this.InternalDocument)
                .WhereElementIsElementType()
                .ToElements()
                .ToList();

            if (elementTypes == null)
                return new List<ElementId>();

            var nonPurgedElements = new HashSet<ElementId>();

            for (int i = 0; i < elementTypes.Count; i++)
            {
                nonPurgedElements.UnionWith(GetUsedMaterialIds(elementTypes[i]));
            }

            List<ElementId> purgeableMaterials = materials.Except(nonPurgedElements).ToList();
            if (purgeableMaterials.Count > 0)
                this.InternalDocument.Delete(purgeableMaterials);

            List<ElementId> purgedMaterialAssets = PurgeMaterialAssets(nonPurgedElements.ToList());

            return purgeableMaterials.Concat(purgedMaterialAssets).ToList();
        }

        private static HashSet<ElementId> GetUsedMaterialIds(Autodesk.Revit.DB.Element type)
        {
            HostObjAttributes hostObj = type as HostObjAttributes;
            HashSet<ElementId> usedMaterialIds = new HashSet<ElementId>();
            if (hostObj != null)
            {
                var compundStructure = hostObj.GetCompoundStructure();
                if (compundStructure != null)
                {
                    int layerCount = compundStructure.LayerCount;
                    for (int j = 0; j < layerCount; j++)
                    {
                        usedMaterialIds.Add(compundStructure.GetMaterialId(j));
                    }
                }
            }
            List<ElementId> elementMaterialIds = type.GetMaterialIds(false).ToList();
            if (elementMaterialIds.Any())
                usedMaterialIds.UnionWith(elementMaterialIds);
            
            return usedMaterialIds;
        }

        private List<Autodesk.Revit.DB.ElementId> PurgeMaterialAssets(List<Autodesk.Revit.DB.ElementId> elementIds)
        {
            List<ElementId> appearanceAssetIds = new FilteredElementCollector(this.InternalDocument)
                .OfClass(typeof(Autodesk.Revit.DB.AppearanceAssetElement))
                .ToElementIds()
                .ToList();

            // The PropertySetElement contains either the Thermal or Structural Asset
            // https://forums.autodesk.com/t5/revit-api-forum/material-assets-collector-appearance-structural-physical-amp/td-p/7256944
            List<ElementId> propertySet = new FilteredElementCollector(this.InternalDocument)
                .OfClass(typeof(PropertySetElement))
                .ToElementIds()
                .ToList();

            int elementIdsCount = elementIds.Count();
            for (int i = 0; i < elementIdsCount; i++)
            {
                try
                {
                    var material = this.InternalDocument.GetElement(elementIds[i]) as Autodesk.Revit.DB.Material;
                    propertySet.Remove(material.ThermalAssetId);
                    propertySet.Remove(material.StructuralAssetId);
                    appearanceAssetIds.Remove(material.AppearanceAssetId);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (appearanceAssetIds.Count > 0)
                this.InternalDocument.Delete(appearanceAssetIds);
            if (propertySet.Count > 0)
                this.InternalDocument.Delete(propertySet);

            return appearanceAssetIds.Concat(propertySet).ToList();
        }

        private List<ElementId> PurgeElements(bool runRecursively)
        {
            List<ElementId> purgedElementIds = new List<ElementId>();
            List<ElementId> purgeableElementIds = GetPurgeableElementIds();

            if (purgeableElementIds == null)
                return purgedElementIds;

            purgedElementIds.AddRange(purgeableElementIds);
            this.InternalDocument.Delete(purgeableElementIds);

            if (runRecursively)
            {
                purgedElementIds.AddRange(PurgeElements(runRecursively));
            }

            return purgedElementIds;
        }

        private List<Autodesk.Revit.DB.ElementId> GetPurgeableElementIds()
        {
            List<Autodesk.Revit.DB.FailureMessage> failureMessages = PerformanceAdviser.GetPerformanceAdviser()
                                                                                       .ExecuteRules(this.InternalDocument, PerformanceAdviserRuleIds)
                                                                                       .ToList();
            if (failureMessages.Count > 0)
            {
                List<ElementId> purgeableElementIds = failureMessages[0].GetFailingElements().ToList();
                return purgeableElementIds;
            }
            return null;
        }

        private static List<PerformanceAdviserRuleId> GetPerfromanceAdviserRuleIdForPurge()
        {
            //The internal GUID of the purge Performance Adviser Rule 
            const string purgePerformanceAdviserRuleGuid = "e8c63650-70b7-435a-9010-ec97660c1bda";

            IList<PerformanceAdviserRuleId> performanceAdviserRules = PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds();
            List<PerformanceAdviserRuleId> performanceAdviserRuleId = performanceAdviserRules.Where(x => x.Guid.ToString() == purgePerformanceAdviserRuleGuid).ToList();
            return performanceAdviserRuleId;
        }

        /// <summary>
        /// Extracts Latitude and Longitude from Revit
        /// </summary>
        /// 
        /// <returns name="Lat">Latitude</returns>
        /// <returns name="Long">Longitude</returns>
        /// <search>Latitude, Longitude</search>

        public DynamoUnits.Location Location
        {
            get
            {
                var loc = InternalDocument.SiteLocation;
                return DynamoUnits.Location.ByLatitudeAndLongitude(
                    loc.Latitude.ToDegrees(),
                    loc.Longitude.ToDegrees());
            }
        }

        /// <summary>
        /// Saves all of the input families at a given location.
        /// </summary>
        /// <param name="family">The Revit family to save</param>
        /// <param name="directoryPath">Directory to save the family to. If directory does not exist, it will be created.</param>
        /// <returns>File path of saved families</returns>
        public string SaveFamilyToFolder(Revit.Elements.Family family, string directoryPath)
        {
            var dir = new DirectoryInfo(directoryPath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Close all transactions
            var trans = TransactionManager.Instance;
            trans.ForceCloseTransaction();

            var fam = family.InternalFamily;
            var familyDocument = this.InternalDocument.EditFamily(fam);
            var name = fam.Name + ".rfa";
            string filePath = Path.Combine(directoryPath, name);
            familyDocument.SaveAs(filePath);
            familyDocument.Close(false);
            return filePath;
        }

        /// <summary>
        /// Provide the Unit type from the associated with the SpecType in the Revit Document.
        /// </summary>
        /// <param name="specType"></param>
        /// <returns></returns>
        public DynamoUnits.Unit UnitTypeBySpecType(ForgeType specType)
        {
            var units = InternalDocument.GetUnits();
            var unitTypeId = units.GetFormatOptions(specType.InternalForgeTypeId).GetUnitTypeId();
            var cleanUnitTypeId = UnitConverter.ConvertRevitInternalUnitTypeIdToSupportedUnitTypeId(unitTypeId);

            return Unit.ByTypeID(cleanUnitTypeId.TypeId);
        }
        /// <summary>
        /// Retrieves all link instances in the current Revit document
        /// </summary>
        /// <param name="document">Document</param>
        /// <returns name="linkInstances[]">List of link instances in the document</returns>
        public List<Elements.Element> GetLinkInstances()
        {
            var links = new FilteredElementCollector(this.InternalDocument)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType()
                .Select(el => el.ToDSType(true))
                .ToList();

            return links;
        }
    }

}

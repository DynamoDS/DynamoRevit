﻿using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace RevitServices.Materials
{
    public class MaterialsManager
    {
        private static MaterialsManager instance;

        public ElementId DynamoMaterialId { get; private set; }
        public ElementId DynamoErrorMaterialId { get; private set; }
        public ElementId DynamoGStyleId { get; private set; }

        public static MaterialsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MaterialsManager();
                }
                return instance;
            }
        }

        private MaterialsManager()
        {
            
        }

        /// <summary>
        /// Finds or creates materials and graphics styles required by the
        /// material manager in the active document. Note that this method 
        /// must be called from Revit idle thread.
        /// </summary>
        public void InitializeForActiveDocumentOnIdle()
        {
            FindOrCreateDynamoMaterial();
            FindOrCreateDynamoErrorMaterial();
            FindDynamoGraphicsStyle();
        }

        public static void Reset()
        {
            instance = null;
            instance = new MaterialsManager();
        }

        private void FindOrCreateDynamoMaterial()
        {
            Dictionary<string, Material> materials
            = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
              .OfClass(typeof(Material))
              .Cast<Material>()
              .ToDictionary<Material, string>(
                e => e.Name);

            // First try to get or create the Dynamo material
            if (materials.ContainsKey("Dynamo"))
            {
                DynamoMaterialId = materials["Dynamo"].Id;
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(
                DocumentManager.Instance.CurrentDBDocument);

            Material glass;
            Material dynamoMaterial;
            if (materials.TryGetValue("Glass", out glass))
            {
                dynamoMaterial = glass.Duplicate("Dynamo");
            }
            else
            {
                var dynamoMaterialId = Material.Create(DocumentManager.Instance.CurrentDBDocument, "Dynamo");
                dynamoMaterial = DocumentManager.Instance.CurrentDBDocument.GetElement(dynamoMaterialId) as Material;
                dynamoMaterial.Transparency = 90;
                dynamoMaterial.UseRenderAppearanceForShading = true;
            }
            dynamoMaterial.Color = new Color(255, 128, 0);
            DynamoMaterialId = dynamoMaterial.Id;

            TransactionManager.Instance.ForceCloseTransaction();
        }

        private void FindOrCreateDynamoErrorMaterial()
        {
            Dictionary<string, Material> materials
            = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
              .OfClass(typeof(Material))
              .Cast<Material>()
              .ToDictionary<Material, string>(
                e => e.Name);

            // First try to get or create the DynamoError material
            if (materials.ContainsKey("DynamoError"))
            {
                DynamoMaterialId = materials["DynamoError"].Id;
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(
                DocumentManager.Instance.CurrentDBDocument);

            var dynamoErrorMaterialId = Material.Create(DocumentManager.Instance.CurrentDBDocument, "DynamoError");
            var dynamoErrorMaterial = DocumentManager.Instance.CurrentDBDocument.GetElement(dynamoErrorMaterialId) as Material;
            dynamoErrorMaterial.Transparency = 95;
            dynamoErrorMaterial.UseRenderAppearanceForShading = true;
            dynamoErrorMaterial.Color = new Color(255, 0, 0);
            DynamoErrorMaterialId = dynamoErrorMaterial.Id;

            TransactionManager.Instance.ForceCloseTransaction();
        }

        private void FindDynamoGraphicsStyle()
        {
            var styles = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            styles.OfClass(typeof(GraphicsStyle));

            var gStyle = styles.ToElements().FirstOrDefault(x => x.Name == "Generic Models");

            if (gStyle != null)
            {
                DynamoGStyleId = gStyle.Id;
            }
            else
            {
                DynamoGStyleId = ElementId.InvalidElementId;
            }
        }
    }
}

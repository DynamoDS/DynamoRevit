﻿using System.Linq;
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
            DynamoMaterialId = ElementId.InvalidElementId;
            DynamoErrorMaterialId = ElementId.InvalidElementId;
            DynamoGStyleId = ElementId.InvalidElementId;
        }

        /// <summary>
        /// Finds or creates materials and graphics styles required by the
        /// material manager in the given document.
        /// </summary>
        public void InitializeForDocument(Document doc)
        {
            FindOrCreateDynamoMaterial(doc);
            FindOrCreateDynamoErrorMaterial(doc);
            FindDynamoGraphicsStyle(doc);
        }

        /// <summary>
        /// Finds or creates materials and graphics styles required by the
        /// material manager in the active document. Note that this method 
        /// must be called from Revit idle thread.
        /// </summary>
        public void InitializeForActiveDocumentOnIdle()
        {
            InitializeForDocument(DocumentManager.Instance.CurrentDBDocument);
        }

        public static void Reset()
        {
            instance = null;
            instance = new MaterialsManager();
        }

        private void FindOrCreateDynamoMaterial(Document doc)
        {
            var materials = new FilteredElementCollector(doc)
                .OfClass(typeof(Material))
                .Cast<Material>();

            // First try to get or create the Dynamo material
            var material = materials.FirstOrDefault(x => x.Name == "Dynamo");
            if (material != null)
            {
                DynamoMaterialId = material.Id;
                return;
            }


            TransactionManager.Instance.EnsureInTransaction(doc);

            Material glassMaterial = materials.FirstOrDefault(x => x.Name == "Glass");
            Material dynamoMaterial;
            if(glassMaterial != null)
            {
                dynamoMaterial = glassMaterial.Duplicate("Dynamo");
            }
            else
            {
                var dynamoMaterialId = Material.Create(doc, "Dynamo");
                dynamoMaterial = doc.GetElement(dynamoMaterialId) as Material;
                dynamoMaterial.Transparency = 90;
                dynamoMaterial.UseRenderAppearanceForShading = true;
            }
            dynamoMaterial.Color = new Color(255, 128, 0);
            DynamoMaterialId = dynamoMaterial.Id;

            TransactionManager.Instance.ForceCloseTransaction();
        }

        private void FindOrCreateDynamoErrorMaterial(Document doc)
        {
            var materials = new FilteredElementCollector(doc)
               .OfClass(typeof(Material))
               .Cast<Material>();


            // First try to get or create the Dynamo material
            var material = materials.FirstOrDefault(x => x.Name == "DynamoError");
            if (material != null)
            {
                DynamoErrorMaterialId = material.Id;
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(doc);

            var dynamoErrorMaterialId = Material.Create(doc, "DynamoError");
            var dynamoErrorMaterial = doc.GetElement(dynamoErrorMaterialId) as Material;
            dynamoErrorMaterial.Transparency = 95;
            dynamoErrorMaterial.UseRenderAppearanceForShading = true;
            dynamoErrorMaterial.Color = new Color(255, 0, 0);
            DynamoErrorMaterialId = dynamoErrorMaterial.Id;

            TransactionManager.Instance.ForceCloseTransaction();
        }

        private void FindDynamoGraphicsStyle(Document doc)
        {
            var genericModelCategory = Category.GetCategory(doc, BuiltInCategory.OST_GenericModel);
            if(genericModelCategory != null)
            {
                var gStyle = genericModelCategory.GetGraphicsStyle(GraphicsStyleType.Projection);
                if(gStyle != null)
                {
                    DynamoGStyleId = gStyle.Id;
                }
            }
        }
    }
}

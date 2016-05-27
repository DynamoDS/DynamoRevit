using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace RevitServices.Materials
{
    public class MaterialsManager
    {
        private static MaterialsManager instance;

        public ElementId DynamoMaterialId { get; private set; } = ElementId.InvalidElementId;
        public ElementId DynamoErrorMaterialId { get; private set; } = ElementId.InvalidElementId;
        public ElementId DynamoGStyleId { get; private set; } = ElementId.InvalidElementId;

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
            var materials = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Material))
                .Cast<Material>();

            // First try to get or create the Dynamo material
            var material = materials.FirstOrDefault(x => x.Name == "Dynamo");
            if (material != null)
            {
                DynamoMaterialId = material.Id;
                return;
            }


            TransactionManager.Instance.EnsureInTransaction(
                DocumentManager.Instance.CurrentDBDocument);

            Material glassMaterial = materials.FirstOrDefault(x => x.Name == "Glass");
            Material dynamoMaterial;
            if(glassMaterial != null)
            {
                dynamoMaterial = glassMaterial.Duplicate("Dynamo");
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
            var materials = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
               .OfClass(typeof(Material))
               .Cast<Material>();


            // First try to get or create the Dynamo material
            var material = materials.FirstOrDefault(x => x.Name == "DynamoError");
            if (material != null)
            {
                DynamoErrorMaterialId = material.Id;
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
            var categories = DocumentManager.Instance.CurrentDBDocument.Settings.Categories;
            var genericModelCategory = categories.get_Item(BuiltInCategory.OST_GenericModel);
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

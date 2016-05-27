using Autodesk.Revit.DB;
using NUnit.Framework;
using RevitTestServices;
using RevitServices.Elements;
using RevitServices.Materials;
using RevitServices.Persistence;
using RevitServices.Transactions;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    internal class MaterialsManagerTests : RevitSystemTestBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            //setup the material manager just for tests
            var mgr = MaterialsManager.Instance;
            mgr.InitializeForActiveDocumentOnIdle();
        }

        [Test]
        [TestModel(@".\\MaterialsManager\DuplicatedMaterialsNoDynamoMaterials.rvt")]
        public void MaterialsManagerDuplicatedMaterialsNoDynamoMaterials()
        {
            Assert.NotNull(MaterialsManager.Instance.DynamoMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoErrorMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoGStyleId);

            Assert.AreNotEqual(MaterialsManager.Instance.DynamoMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoErrorMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoGStyleId, ElementId.InvalidElementId);
        }

        [Test]
        [TestModel(@".\\MaterialsManager\DuplicatedMaterialsWithDynamoMaterials.rvt")]
        public void MaterialsManagerDuplicatedMaterialsWithDynamoMaterials()
        {
            Assert.NotNull(MaterialsManager.Instance.DynamoMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoErrorMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoGStyleId);

            Assert.AreNotEqual(MaterialsManager.Instance.DynamoMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoErrorMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoGStyleId, ElementId.InvalidElementId);
        }
    }
}

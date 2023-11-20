using System;
using System.IO;
using Autodesk.Revit.DB;
using Newtonsoft.Json;
using NUnit.Framework;
using RevitServices.Elements;
using RevitServices.Materials;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace RevitServicesTests
{
    [TestFixture]
    public class RevitServicesTests
    {
        Document Document
        {
            get { return DocumentManager.Instance.CurrentUIDocument.Document; }
        }

        [SetUp]
        public void Setup()
        {
            DocumentManager.Instance.CurrentUIApplication =
                RTF.Applications.RevitTestExecutive.CommandData.Application;
            DocumentManager.Instance.CurrentUIDocument =
                RTF.Applications.RevitTestExecutive.CommandData.Application.ActiveUIDocument;
        }

        [Test]
        [Category("UnitTests")]
        public void MakePoint()
        {
            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            var t = transManager.StartTransaction(Document);

            var id = Document.FamilyCreate.NewReferencePoint(new XYZ(0, 0, 0)).Id;

            t.CommitTransaction();

            ReferencePoint rp;
            Assert.IsTrue(Document.TryGetElement(id, out rp));
            Assert.AreEqual(id, rp.Id);
        }

        [Test]
        [Category("UnitTests")]
        public void MakePointThenCancel()
        {
            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            var t = transManager.StartTransaction(Document);

            var id = Document.FamilyCreate.NewReferencePoint(new XYZ(0, 0, 0)).Id;

            t.CancelTransaction();

            ReferencePoint rp;
            Assert.IsFalse(Document.TryGetElement(id, out rp));
        }

        [Test]
        [Category("UnitTests")]
        public void TransactionStartedEventFires()
        {
            bool eventWasFired = false;

            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            transManager.TransactionStarted += delegate { eventWasFired = true; };

            Assert.IsFalse(eventWasFired);
            var t = transManager.StartTransaction(Document);

            Assert.IsTrue(eventWasFired);
            t.CancelTransaction();
        }

        [Test]
        [Category("UnitTests")]
        public void TransactionCommittedEventFires()
        {
            bool eventWasFired = false;

            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            transManager.TransactionCommitted += delegate { eventWasFired = true; };

            Assert.IsFalse(eventWasFired);

            var t = transManager.StartTransaction(Document);
            Assert.IsFalse(eventWasFired);

            t.CommitTransaction();
            Assert.IsTrue(eventWasFired);
        }

        [Test]
        [Category("UnitTests")]
        public void TransactionCancelledEventFires()
        {
            bool eventWasFired = false;

            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            transManager.TransactionCancelled += delegate { eventWasFired = true; };

            Assert.IsFalse(eventWasFired);

            var t = transManager.StartTransaction(Document);
            Assert.IsFalse(eventWasFired);

            t.CancelTransaction();
            Assert.IsTrue(eventWasFired);
        }

        [Test]
        [Category("UnitTests")]
        public void TransactionActive()
        {
            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;
            Assert.IsFalse(transManager.TransactionActive);

            var t = transManager.StartTransaction(Document);
            Assert.IsTrue(transManager.TransactionActive);

            t.CancelTransaction();
            Assert.IsFalse(transManager.TransactionActive);

            t = transManager.StartTransaction(Document);
            Assert.IsTrue(transManager.TransactionActive);

            t.CommitTransaction();
            Assert.IsFalse(transManager.TransactionActive);
        }

        [Test]
        [Category("UnitTests")]
        public void TransactionHandleStatus()
        {
            TransactionManager.SetupManager();
            var transManager = TransactionManager.Instance.TransactionWrapper;

            var t = transManager.StartTransaction(Document);
            Assert.AreEqual(TransactionStatus.Started, t.Status);

            t.CommitTransaction();

            Assert.Throws<InvalidOperationException>(
                () => t.CommitTransaction(), 
                "Cannot commit a transaction that isn't active.");

            Assert.Throws<InvalidOperationException>(
                () => t.CancelTransaction(),
                "Cannot cancel a transaction that isn't active.");
        }

        [Test]
        [Category("UnitTests")]
        public void FailuresRaisedEvent()
        {
            //TODO
            Assert.Inconclusive("TODO: find an example that would cause revit to emit failures");
        }

        [Test]
        [Category("UnitTests")]
        public void TestRoundTripElementSerialisation()
        {
            // Create an instance of the type and serialize it.
            var elementId = new SerializableId
            {
                IntID = 42,
                StringID = "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"
            };

            //Readback
            var readback = JsonConvert.DeserializeObject<SerializableId>(JsonConvert.SerializeObject(elementId));

            Assert.IsTrue(readback.IntID == 42);
            Assert.IsTrue(readback.StringID.Equals("{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"));
        }

        [Test]
        [Category("UnitTests")]
        public void RawTraceInteractionTest()
        {
            var elementId = new SerializableId
            {
                IntID = 42,
                StringID = "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"
            };

            //Raw write
            ElementBinder.SetRawDataForTrace(JsonConvert.SerializeObject(elementId));
            elementId = null;

            //Readback
            elementId = JsonConvert.DeserializeObject<SerializableId>( ElementBinder.GetRawDataFromTrace());
            Assert.IsTrue(elementId.IntID == 42);
            Assert.IsTrue(elementId.StringID == "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}");
        }

        [Test]
        [Category("UnitTests")]
        public void TestRoundTripElementSerialisationForMultipleIDs()
        {
            // Create an instance of the type and serialize it.
            var elementIDs = new MultipleSerializableId
            {
                IntIDs = { 42, 20 },
                StringIDs = { "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}", "{A79294BF-6D27-4B86-BEE9-D6921C11D495}" }
            };

            //Readback
            var readback = JsonConvert.DeserializeObject<MultipleSerializableId>(JsonConvert.SerializeObject(elementIDs));


            //Verify that the data is correct
            Assert.IsTrue(readback.IntIDs.Count == 2);
            Assert.IsTrue(readback.IntIDs[0] == 42);
            Assert.IsTrue(readback.IntIDs[1] == 20);
            Assert.IsTrue(readback.StringIDs.Count == 2);
            Assert.IsTrue(readback.StringIDs[0].Equals("{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"));
            Assert.IsTrue(readback.StringIDs[1].Equals("{A79294BF-6D27-4B86-BEE9-D6921C11D495}"));
        }

        [Test]
        [Category("UnitTests")]
        public void RawTraceInteractionTestWithMultipleIDs()
        {
            var elementIDs = new MultipleSerializableId
            {
                IntIDs = { 42, 20 },
                StringIDs = { "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}", "{A79294BF-6D27-4B86-BEE9-D6921C11D495}" }
            };

            //Raw write
            ElementBinder.SetRawDataForTrace(JsonConvert.SerializeObject(elementIDs));
            elementIDs = null;

            //Readback
            var readback = JsonConvert.DeserializeObject<MultipleSerializableId>(ElementBinder.GetRawDataFromTrace());

            //Verify that the data is correct
            Assert.IsTrue(readback.IntIDs.Count == 2);
            Assert.IsTrue(readback.IntIDs[0] == 42);
            Assert.IsTrue(readback.IntIDs[1] == 20);
            Assert.IsTrue(readback.StringIDs.Count == 2);
            Assert.IsTrue(readback.StringIDs[0].Equals("{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"));
            Assert.IsTrue(readback.StringIDs[1].Equals("{A79294BF-6D27-4B86-BEE9-D6921C11D495}"));
        }

        [Test]
        [Category("UnitTests")]
        public void TwoInstancesOfMultSerializableIdAreEqual()
        {
            var elementIDs = new MultipleSerializableId
            {
                IntIDs = { 42, 20 },
                StringIDs = { "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}", "{A79294BF-6D27-4B86-BEE9-D6921C11D495}" }
            };

            var elementIDs2 = new MultipleSerializableId
            {
                IntIDs = { 42, 20 },
                StringIDs = { "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}", "{A79294BF-6D27-4B86-BEE9-D6921C11D495}" }
            };

            //Raw write
            ElementBinder.SetRawDataForTrace(JsonConvert.SerializeObject(elementIDs));
            elementIDs = null;

            //Readback
            var readback = JsonConvert.DeserializeObject<MultipleSerializableId>( ElementBinder.GetRawDataFromTrace());

            //verify that the id extracted from trace is equal to the new instance
            Assert.IsTrue(readback.Equals(JsonConvert.SerializeObject(elementIDs2)));

        }

        [Test]
        [Category("UnitTests")]
        public void TwoInstancesOfSerializableIdAreEqual()
        {
            var elementID = new SerializableId
            {
                IntID = 42,
            StringID = "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"
            };

            var elementID2 = new SerializableId
            {
                IntID = 42,
                StringID = "{BE507CAC-7F23-43D6-A2B4-13F6AF09046F}"
            };

            //Raw write
            ElementBinder.SetRawDataForTrace(JsonConvert.SerializeObject(elementID));
            elementID = null;

            //Readback
            var readback = JsonConvert.DeserializeObject<SerializableId>(ElementBinder.GetRawDataFromTrace());

            //verify that the id extracted from trace is equal to the new instance
            Assert.IsTrue(readback.Equals(JsonConvert.SerializeObject(elementID2)));

        }

        [Test]
        [Category("UnitTests")]
        public void MaterialManagerCheckDefaultValues()
        {
            Assert.NotNull(MaterialsManager.Instance.DynamoMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoErrorMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoGStyleId);

            Assert.AreEqual(MaterialsManager.Instance.DynamoMaterialId, ElementId.InvalidElementId);
            Assert.AreEqual(MaterialsManager.Instance.DynamoErrorMaterialId, ElementId.InvalidElementId);
            Assert.AreEqual(MaterialsManager.Instance.DynamoGStyleId, ElementId.InvalidElementId);
        }

        [Test]
        [Category("UnitTests")]
        public void MaterialManagerCheckValues()
        {
            var mgr = MaterialsManager.Instance;
            mgr.InitializeForActiveDocumentOnIdle();

            Assert.NotNull(MaterialsManager.Instance.DynamoMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoErrorMaterialId);
            Assert.NotNull(MaterialsManager.Instance.DynamoGStyleId);

            Assert.AreNotEqual(MaterialsManager.Instance.DynamoMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoErrorMaterialId, ElementId.InvalidElementId);
            Assert.AreNotEqual(MaterialsManager.Instance.DynamoGStyleId, ElementId.InvalidElementId);
        }

    }
}

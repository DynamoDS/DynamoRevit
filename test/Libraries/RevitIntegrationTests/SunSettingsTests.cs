using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using DSRevitNodesUI;

using Dynamo.Models;

using NUnit.Framework;

using Revit.GeometryConversion;

using RevitServices.Persistence;

using RTF.Framework;

using RevitTestServices;

namespace RevitSystemTests
{
    [TestFixture]
    class SunSettingsTests : RevitSystemTestBase
    {

        [Test, TestModel(@".\SunSettings\SunSettings.rvt")]
        public void SiteLocation_ValidArgs()
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;

            OpenAndRunDynamoDefinition(@".\SunSettings\SunSettings.dyn");

            var sunNode =
                ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is SunSettings);

            SetSunSettings(doc, 29.2838918336686, -179.038560260629);// current data from SunSettings.rvt
            RunCurrentModel();
            AssertSunSettingsValues(sunNode, doc);


        }

        private void AssertSunSettingsValues(NodeModel sunNode, Document doc)
        {
            Revit.Elements.SunSettings sunValue = GetPreviewValue(sunNode.GUID.ToString()) as Revit.Elements.SunSettings;
            //Assert.AreEqual(sunValue.Name, doc.SiteLocation.PlaceName);
            Assert.AreEqual(sunValue.Altitude, doc.SiteLocation.Latitude.ToDegrees(), 0.001);
            Assert.AreEqual(sunValue.Azimuth, doc.SiteLocation.Longitude.ToDegrees(), 0.001);
        }


        private static void SetSunSettings(Document doc, double altitude, double azimuth)
        {
            using (var t = new Transaction(doc))
            {
                t.Start("Set Sun Setting.");
                doc.ActiveView.SunAndShadowSettings.Altitude = altitude.ToRadians();
                doc.ActiveView.SunAndShadowSettings.Azimuth = azimuth.ToRadians();

                t.Commit();
            }
        }
    }
}

using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class SunSettingsTests : RevitNodeTestBase
    {
        [Test, TestModel(@".\Empty.rvt")]
        public void Current()
        {
            Assert.AreEqual(DocumentManager.Instance.CurrentDBDocument.ActiveView.
                SunAndShadowSettings.Id, SunSettings.Current().InternalSunAndShadowSettings.Id);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void Direction()
        {
            Vector.ByCoordinates(39.898085, -28.624307, 87.113672).ShouldBeApproximately(
                SunSettings.Current().SunDirection);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void Altitude()
        {
            SunSettings.Current().Altitude.ShouldBeApproximately(60.591003);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void Azimuth()
        {
            SunSettings.Current().Azimuth.ShouldBeApproximately(125.657004);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StartDateTime()
        {
            Assert.AreEqual(
                SunSettings.TranslateTime(
                DocumentManager.Instance.CurrentDBDocument.ActiveView.SunAndShadowSettings.StartDateAndTime),
                SunSettings.Current().StartDateTime);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void EndDateTime()
        {
            Assert.AreEqual(
                SunSettings.TranslateTime(
                DocumentManager.Instance.CurrentDBDocument.ActiveView.SunAndShadowSettings.EndDateAndTime),
                SunSettings.Current().EndDateTime);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void CurrentDateTime()
        {
            Assert.AreEqual(
                SunSettings.TranslateTime(
                DocumentManager.Instance.CurrentDBDocument.ActiveView.SunAndShadowSettings.ActiveFrameTime),
                SunSettings.Current().CurrentDateTime);
        }
    }
}

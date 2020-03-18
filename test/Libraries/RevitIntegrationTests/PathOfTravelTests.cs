using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using CoreNodeModels.Input;
using Dynamo.Nodes;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System;
using System.IO;
using System.Linq;

namespace RevitSystemTests
{
   [TestFixture]
   class PathOfTravelTests : RevitSystemTestBase
   {
      [Test]
      [TestModel(@".\PathOfTravel\RoomsAndExits.rvt")]
      public void CreateLongestPathOfTravels()
      {
         string samplePath = Path.Combine(workingDirectory, @".\PathOfTravel\CalculateLongestExitDistance.dyn");
         string testPath = Path.GetFullPath(samplePath);

         ViewModel.OpenCommand.Execute(testPath);

         RunCurrentModel();

         Document doc = DocumentManager.Instance.CurrentDBDocument;
         var collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PathOfTravelLines);

         // Two longest paths of ravel are created in this test
         Assert.AreEqual(2, collector.ToElements().Count());
      }
   }
}
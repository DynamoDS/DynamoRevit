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

         // Two longest paths of travel are created in this test
         Assert.AreEqual(2, collector.ToElements().Count());
      }

      [Test]
      [TestModel(@".\PathOfTravel\PathOfTravelsAndWayPoints.rvt")]
      public void WayPointsGetInsertSetRemove()
      {
         string samplePath = Path.Combine(workingDirectory, @".\PathOfTravel\GetInsertSetRemoveWayPoints.dyn");
         string testPath = Path.GetFullPath(samplePath);

         ViewModel.OpenCommand.Execute(testPath);

         RunCurrentModel();

         // Node "Left WayPoints After insertion"
         var wayPointsAfterinsertion = GetFlattenedPreviewValues("f128cb04b2f74fb2ac27232963885af4");
         Assert.IsNotNull(wayPointsAfterinsertion);
         Assert.AreEqual(1, wayPointsAfterinsertion.Count);
         Assert.IsNotNull(wayPointsAfterinsertion[0]);
         Assert.AreEqual("(-3.669000000, 15.000000000, 0.000000000)", wayPointsAfterinsertion[0].ToString());

         // Node "Right initial WayPoints"
         var rightinitialWayPoints = GetFlattenedPreviewValues("185cdb48e36c435abb08847df842ac9c");
         Assert.IsNotNull(rightinitialWayPoints);
         Assert.AreEqual(2, rightinitialWayPoints.Count);
         Assert.IsNotNull(rightinitialWayPoints[0]);
         Assert.IsNotNull(rightinitialWayPoints[1]);
         Assert.AreEqual("(51.764361580, 16.711799679, 0.000000000)", rightinitialWayPoints[0].ToString());
         Assert.AreEqual("(42.646072483, 3.614468449, 0.000000000)", rightinitialWayPoints[1].ToString());

         // Nodes "Right WayPoints after set",
         var rightWayPointsAfterSet = GetFlattenedPreviewValues("fa775bfa31034b429aaa8ac49e4b3738");
         Assert.IsNotNull(rightWayPointsAfterSet);
         Assert.AreEqual(2, rightWayPointsAfterSet.Count);
         Assert.IsNotNull(rightWayPointsAfterSet[0]);
         Assert.IsNotNull(rightWayPointsAfterSet[1]);
         Assert.AreEqual("(56.372000000, 23.331000000, 0.000000000)", rightWayPointsAfterSet[0].ToString());
         Assert.AreEqual("(42.646072483, 3.614468449, 0.000000000)", rightWayPointsAfterSet[1].ToString());

         // Node "Right WayPoints after removal",
         var wayPointsAfterRemoval = GetFlattenedPreviewValues("b0d8c3f4aefd412db29a2ee6aed80cd6");
         Assert.IsNotNull(wayPointsAfterRemoval);
         Assert.AreEqual(1, wayPointsAfterRemoval.Count);
         Assert.IsNotNull(wayPointsAfterRemoval[0]);
         Assert.AreEqual("(56.372000000, 23.331000000, 0.000000000)", wayPointsAfterRemoval[0].ToString());
      }
   }
}
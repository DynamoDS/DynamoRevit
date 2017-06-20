using System;
using System.IO;
using System.Linq;
using Dynamo.Graph.Nodes;
using Dynamo.Nodes;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class MigrationTest : RevitSystemTestBase
    {
        private void TestMigration(string filename)
        {
            string testPath = Path.Combine(workingDirectory, filename);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var nodes = ViewModel.Model.CurrentWorkspace.Nodes;
            var unresolvedNodeCount = 0;
            var str = "\n";

            foreach (var node in nodes.OfType<DummyNode>())
            {
                if (node.NodeNature == DummyNode.Nature.Unresolved) 
                {
                    unresolvedNodeCount++;
                    str += node.Name;
                    str += "\n";
                }
            }

            if (unresolvedNodeCount >= 1)
            {
                Assert.Fail("Number of unresolved nodes found in TestCase: " + unresolvedNodeCount +str);
            }
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Analyze_Daylighting()
        {
            TestMigration(@".\Migration\TestMigration_Analyze_Daylighting.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Analyze_Display()
        {
            TestMigration(@".\Migration\TestMigration_Analyze_Display.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Analyze_Measure()
        {
            TestMigration(@".\Migration\TestMigration_Analyze_Measure.dyn");
        }

        [Test, Category("Failure")]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Analyze_Render()
        {
            TestMigration(@".\Migration\TestMigration_Analyze_Render.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Analyze_Solar()
        {
            TestMigration(@".\Migration\TestMigration_Analyze_Solar.dyn");
        }

        [Test, Category("Failure")]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Geometry_Curve()
        {
            TestMigration(@".\Migration\TestMigration_Geometry_Curve.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Geometry_Intersect()
        {
            TestMigration(@".\Migration\TestMigration_Geometry_Intersect.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Geometry_Point()
        {
            TestMigration(@".\Migration\TestMigration_Geometry_Point.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Geometry_Solid()
        {
            TestMigration(@".\Migration\TestMigration_Geometry_Solid.dyn");
        }

        [Test, Category("Failure")]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Geometry_Surface()
        {
            TestMigration(@".\Migration\TestMigration_Geometry_Surface.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_API()
        {
            TestMigration(@".\Migration\TestMigration_Revit_API.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_Datums()
        {
            TestMigration(@".\Migration\TestMigration_Revit_Datums.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_Document()
        {
            TestMigration(@".\Migration\TestMigration_Revit_Document.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_Families()
        {
            TestMigration(@".\Migration\TestMigration_Revit_Families.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_Referemce()
        {
            TestMigration(@".\Migration\TestMigration_Revit_Referemce.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_Selection()
        {
            TestMigration(@".\Migration\TestMigration_Revit_Selection.dyn");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_View()
        {
            TestMigration(@".\Migration\TestMigration_Revit_View.dyn");
        }
        [Test]
        [TestModel(@".\empty.rfa")]
        public void TestMigration_Revit_FamilySymbolToFamilyType()
        {
            TestMigration(@".\Migration\TestMigration_Revit_FamilyType.dyn");
        }
    }
}

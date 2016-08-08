using System.Linq;

using Autodesk.Revit.DB;

using Revit.Elements;
using NUnit.Framework;
using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using Family = Revit.Elements.Family;
using FamilyType = Revit.Elements.FamilyType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FaceWallTests : RevitNodeTestBase
    {


        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByFace()
        {
            // obtain the element id for the box family
            var name = "Box";

            // look up the loaded family
            var family = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Family>()
                                                      .FirstOrDefault(x => x.Name == name);
            Assert.NotNull(family);
            
            Face face = null;

            foreach (Solid solid in family.get_Geometry(new Options(){ }))
            {
                face = solid.Faces.get_Item(0);
            }

            Assert.IsNotNull(face);
       
            var faceRef = Revit.GeometryReferences.ElementFaceReference.FromExisting(face);
            Assert.IsNotNull(faceRef);

            var wallType = Revit.Elements.WallType.ByName( "Curtain Wall 1" );
            Assert.IsNotNull(wallType);

            var wall = Revit.Elements.FaceWall.ByFace(Autodesk.Revit.DB.WallLocationLine.CoreCenterline, wallType,faceRef);

            Assert.IsNotNull(wall);
            Assert.IsTrue(wall.GetType() == typeof(Revit.Elements.FaceWall));
        }

    }
}


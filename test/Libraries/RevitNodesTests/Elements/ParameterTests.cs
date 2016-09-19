using System;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using NUnit.Framework;
using System.Collections.Generic;
using RevitTestServices;
using RevitServices.Persistence;
using System.Linq;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ParameterTests : RevitNodeTestBase
    {
        /// <summary>
        /// Create a Project parameter and test if it exists
        /// </summary>
        [Test]
        [TestModel(@".\elementSharedParameters.rvt")]
        public void CreateProjectParameter_ValidArgs()
        {
            List<Category> categories = new List<Category>() { Category.ByName("Walls") };

            Parameter.CreateProjectParameter("MyParameter", "MyGroup", Autodesk.Revit.DB.ParameterType.Text.ToString(), Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA.ToString(), true, categories);

            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(Autodesk.Revit.DB.Wall));
            var wall = fec.FirstElement(); 
            Assert.IsNotNull(wall.LookupParameter("MyParameter"));
        }

        /// <summary>
        /// Create a Shared parameter and test if it exists
        /// </summary>
        [Test]
        [TestModel(@".\elementSharedParameters.rvt")]
        public void CreateSharedParameter_ValidArgs()
        {
            List<Category> categories = new List<Category>() { Category.ByName("Walls") };
            //DocumentManager.Instance.CurrentDBDocument
            Parameter.CreateSharedParameter("MySharedParameter", "MySharedGroup", Autodesk.Revit.DB.ParameterType.Text.ToString(), Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA.ToString(), true, categories);
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(Autodesk.Revit.DB.Wall));
            var wall = fec.FirstElement();    
            Assert.IsNotNull(wall.LookupParameter("MySharedParameter"));
        }

        /// <summary>
        /// Set a parameters value
        /// </summary>
        [Test]
        [TestModel(@".\element.rvt")]
        public void SetParameterValue_ValidArgs()
        {
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(Autodesk.Revit.DB.Wall));
            var wall = fec.FirstElement(); 

            var parameter = wall.LookupParameter("Comments");

            Assert.IsNotNull(parameter);

            Parameter DSParam = new Parameter(parameter);

            Parameter.SetValue(DSParam, "Hello World");

            Assert.AreEqual(DSParam.Value, "Hello World");
        }

    }
}


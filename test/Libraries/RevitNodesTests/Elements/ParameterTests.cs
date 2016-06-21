using System;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using NUnit.Framework;
using System.Collections.Generic;
using RevitTestServices;
using RevitServices.Persistence;

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
        [TestModel(@".\empty.rfa")]
        public void CreateProjectParameter_ValidArgs()
        {
            List<Category> categories = new List<Category>() { Category.ByName("Walls") };

            Parameter.CreateProjectParameter("MyParameter", "MyGroup", Autodesk.Revit.DB.ParameterType.Text.ToString(), Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA.ToString(), false, categories);

            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(WallType));
            var element = collector.FirstElement(); 
            Assert.IsNotNull(element.LookupParameter("MyParameter"));
        }

        /// <summary>
        /// Create a Shared parameter and test if it exists
        /// </summary>
        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateSharedParameter_ValidArgs()
        {
            List<Category> categories = new List<Category>() { Category.ByName("Walls") };

            Parameter.CreateSharedParameter("MySharedParameter", "MySharedGroup", Autodesk.Revit.DB.ParameterType.Text.ToString(), Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA.ToString(), false, categories);

            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(WallType));
            var element = collector.FirstElement();          
            Assert.IsNotNull(element.LookupParameter("MySharedParameter"));
        }

        /// <summary>
        /// Set a parameters value
        /// </summary>
        [Test]
        [TestModel(@".\empty.rfa")]
        public void SetParameterValue_ValidArgs()
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(typeof(WallType));
            var element = collector.FirstElement();

            var parameter = element.LookupParameter("Description");
            
            Parameter DSParam = new Parameter(parameter);

            Parameter.SetValue(DSParam, "Hello World");

            Assert.AreEqual(DSParam.Value, "Hello World");
            Assert.AreEqual(parameter.AsValueString(), "Hello World");
        }

    }
}


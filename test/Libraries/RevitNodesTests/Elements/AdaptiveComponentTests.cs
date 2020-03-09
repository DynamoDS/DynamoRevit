﻿using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryConversion;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using FamilyInstance = Autodesk.Revit.DB.FamilyInstance;
using FamilyType = Revit.Elements.FamilyType;
using Form = Revit.Elements.Form;
using ModelCurve = Revit.Elements.ModelCurve;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class AdaptiveComponentTests : RevitNodeTestBase
    {

        public List<XYZ> GetInternalPoints(FamilyInstance adaptiveComponent)
        {
            var pts = new List<XYZ>();
            var ids = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(adaptiveComponent);
            foreach (var id in ids)
            {
                var p = DocumentManager.Instance.CurrentDBDocument.GetElement(id) as Autodesk.Revit.DB.ReferencePoint;
                pts.Add(p.Position);
            }
            return pts;
        }

        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByPoints_PointArray_ProducesValidAdaptiveComponentAndLocations()
        {
            var pts = new Point[][]
            {
                new Point[]
                {
                    Point.ByCoordinates(0, 0, 0),
                    Point.ByCoordinates(10, 0, 10),
                    Point.ByCoordinates(20, 0, 0)
                }
            };
            var fs = FamilyType.ByName("3PointAC");
            var ac = AdaptiveComponent.ByPoints(pts, fs);

            var locs = ac.First().Locations;

            var pairs = locs.Zip(pts.First(), (point, point1) => new Tuple<Point, Point>(point, point1));

            // compares after unit conversion
            foreach (var pair in pairs)
                pair.Item1.ShouldBeApproximately(pair.Item2);

            var unconvertedPairs = pts.First().Zip(GetInternalPoints((FamilyInstance) ac.First().InternalElement), 
                (point, point1) => new Tuple<Point, XYZ>(point, point1));

            foreach (var pair in unconvertedPairs)
            {
                pair.Item1.ShouldBeApproximately(pair.Item2 * UnitConverter.HostToDynamoFactor(SpecTypeId.Length));
            }

            Assert.NotNull(ac);
        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByPoints_ShouldThrowExceptionWithNonMatchingNumberOfPoints()
        {
            var pts = new Point[][]
            {
                new Point[]
                {
                    Point.ByCoordinates(0, 0, 0),
                    Point.ByCoordinates(10, 0, 10)
                }
            };
            var ft = FamilyType.ByName("3PointAC");

            Assert.Throws(typeof(ArgumentException), () => AdaptiveComponent.ByPoints(pts, ft));
        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByPoints_NullFamilySymbol()
        {
            var pts = new Point[][]
            {
                new Point[]
                {
                    Point.ByCoordinates(0, 0, 0),
                    Point.ByCoordinates(10, 0, 10),
                    Point.ByCoordinates(20, 0, 0)
                }
            };

            Assert.Throws(typeof(ArgumentNullException), () => AdaptiveComponent.ByPoints(pts, null));
        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByPoints_NullPts()
        {
            var ft = FamilyType.ByName("3PointAC");

            Assert.Throws(typeof(ArgumentNullException), () => AdaptiveComponent.ByPoints(null, ft));
        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByPointsOnCurve_ProducesValidAdaptiveComponentAndLocations()
        {
            // create spline
            var pts = new Autodesk.DesignScript.Geometry.Point[]
            {
                Point.ByCoordinates(0,0,0),
                Point.ByCoordinates(1,0,0),
                Point.ByCoordinates(3,0,0),
                Point.ByCoordinates(10,0,0),
                Point.ByCoordinates(12,0,0),
            };

            var spline = NurbsCurve.ByControlPoints(pts, 3);
            Assert.NotNull(spline);

            // build model curve from spline
            var modCurve = ModelCurve.ByCurve(spline);
            Assert.NotNull(modCurve);

            // obtain the family from the document
            var ft = FamilyType.ByName("3PointAC");

            // build the AC
            var parms = new double[]{0, 0.5, 1};

            var ac = AdaptiveComponent.ByParametersOnCurveReference(parms, modCurve, ft);

            // with unit conversion
            foreach (var pt in ac.Locations)
                spline.DistanceTo(pt).ShouldBeApproximately(0);

            // without unit conversion
            var unconvertedPoints = GetInternalPoints((FamilyInstance)ac.InternalElement);

            foreach (var pt in unconvertedPoints)
            {
                spline.DistanceTo(pt.ToPoint()).ShouldBeApproximately(0);
            }

            Assert.NotNull(ac);

        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ByParametersOnFace_CreatesValidACFromElementFaceReference()
        {
            var ele = ElementSelector.ByType<Autodesk.Revit.DB.Form>(true).FirstOrDefault();
            Assert.NotNull(ele);

            var form = ele as Form;
            var faces = form.ElementFaceReferences;
            Assert.IsTrue(faces.All(x => x != null));
            Assert.AreEqual(6, faces.Length);

            var ft = FamilyType.ByName("3PointAC");

            var uvs = new[]
            {
                Autodesk.DesignScript.Geometry.UV.ByCoordinates(0, 0),
                Autodesk.DesignScript.Geometry.UV.ByCoordinates(0.5, 0.5),
                Autodesk.DesignScript.Geometry.UV.ByCoordinates(0.5, 0)
            };

            var ac = AdaptiveComponent.ByParametersOnFace(uvs, faces.First(), ft);

            Assert.NotNull(ac);
        }
    }
}

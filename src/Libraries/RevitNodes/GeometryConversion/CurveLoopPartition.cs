using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using BoundingBox = Autodesk.DesignScript.Geometry.BoundingBox;
using PolyCurve = Autodesk.DesignScript.Geometry.PolyCurve;

namespace Revit.GeometryConversion
{

    [SupressImportIntoVM]
    internal static class CurveLoopPartition
    {
        public static List<CurvePartition> ByCurveLoops(List<List<Curve>> curveLoops)
        {
            if (curveLoops.Count == 1)
                return new List<CurvePartition>()
                {
                    new CurvePartition(){ OuterCurves = curveLoops[0] }
                };

            var tesselatedCurveLoops = new List<Tuple<BoundingBox, List<XYZ>, List<Curve>>>();
            foreach (var curveLoop in curveLoops)
            {
                var verts = TesselateCurveLoop(curveLoop).ToList();
                List<Autodesk.DesignScript.Geometry.Curve> curves =
                    new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (var curve in curveLoop)
                {
                    curves.Add(curve.ToProtoType(false));
                }
                var geom = new List<Autodesk.DesignScript.Geometry.Geometry>();
                geom.Add(PolyCurve.ByJoinedCurves(curves));
                tesselatedCurveLoops.Add(new Tuple<BoundingBox, List<XYZ>, List<Curve>>(BoundingBox.ByGeometry(geom), verts, curveLoop));
                curves.ForEach(x => x.Dispose());
                curves.Clear();
                geom.ForEach(x => x.Dispose());
                geom.Clear();
            }

            var outerCurveLoops = new List<Tuple<BoundingBox, List<XYZ>, List<Curve>>>();
            var innerCurveLoops = new List<Tuple<BoundingBox, List<XYZ>, List<Curve>>>();

            // collect the outer loops, which are not contained in any other loop
            for (int i = 0, j = tesselatedCurveLoops.Count - 1; i < tesselatedCurveLoops.Count; j = i++)
            {
                var curveLoop = tesselatedCurveLoops[i];
                var isOuter = tesselatedCurveLoops.Where((_, l) => i != l)
                    .All(bound => !curveLoop.Item2.IsContainedIn(bound.Item1));

                if (isOuter)
                {
                    outerCurveLoops.Add(curveLoop);
                }
                else
                {
                    innerCurveLoops.Add(curveLoop);
                }
            }

            var curvesPartitions = new List<CurvePartition>();

            // for each outer loop, collect the loops that are inside of it
            // forming the partitions of the original curve loop set
            foreach (var outerLoop in outerCurveLoops)
            {
                var comp = new List<List<Curve>> { outerLoop.Item3 };
                var mask = new List<bool>();
                var curvePatition = new CurvePartition();
                curvePatition.OuterCurves.AddRange(outerLoop.Item3);
                foreach (var innerLoop in innerCurveLoops)
                {
                    if (innerLoop.Item2.IsContainedIn(outerLoop.Item1))
                    {
                        mask.Add(false);
                        comp.Add(innerLoop.Item3);
                        curvePatition.InnerCurves.Add(innerLoop.Item3);
                    }
                    else
                    {
                        mask.Add(true);
                    }
                }
                // remove innerEdge loops that have already been added to a partition
                innerCurveLoops = innerCurveLoops.Where((_, j) => mask[j]).ToList();
                // add the new partition
                curvesPartitions.Add(curvePatition);
            }

            return curvesPartitions;
        }

        public static List<List<Autodesk.Revit.DB.Curve>> GetAllCurveloopsFromRevitFace(Face face)
        {
            var curveLoops = face.GetEdgesAsCurveLoops();
            List<List<Autodesk.Revit.DB.Curve>> curves = new List<List<Curve>>();
            foreach (var curveloop in curveLoops)
            {
                var a = curveloop.GetCurveLoopIterator();
                bool hasItems = false;
                List<Curve> curve = new List<Curve>();
                do
                {
                    hasItems = a.MoveNext();
                    if (hasItems)
                    {
                        var item = a.Current;
                        curve.Add(item);
                    }
                } while (hasItems);
                curves.Add(curve);
            }
            return curves;
        }

        private static IEnumerable<Autodesk.Revit.DB.XYZ> TesselateCurveLoop(IEnumerable<Autodesk.Revit.DB.Curve> curveLoop)
        {
            return curveLoop.SelectMany(x => x.Tessellate().TakeAllButLast());
        }

        private static bool IsContainedIn(this IEnumerable<XYZ> curveLoop, BoundingBox boundingBox)
        {
            return curveLoop.All(x => boundingBox.Contains(x.ToPoint(false)));
        }
    }

    internal class CurvePartition
    {
        public List<Curve> OuterCurves;
        public List<List<Curve>> InnerCurves;

        public CurvePartition()
        {
            OuterCurves = new List<Curve>();
            InnerCurves = new List<List<Curve>>();
        }

        public List<List<Curve>> GetAllCurves()
        {
            var allCurves = new List<List<Curve>>();
            allCurves.Add(OuterCurves);
            allCurves.AddRange(InnerCurves);
            return allCurves;
        }
    }
}

using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryReferences;

using Curve = Autodesk.Revit.DB.Curve;
using Face = Autodesk.Revit.DB.Face;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.GeometryConversion
{
    [SupressImportIntoVM]
    public static class RevitToProtoFace
    {
        public static IEnumerable<Surface> ToProtoType(this Autodesk.Revit.DB.Face revitFace,
          bool performHostUnitConversion = true, Reference referenceOverride = null)
        {
            if (revitFace == null) throw new ArgumentNullException("revitFace");

            var revitCurveLoops = CurveLoopPartition.GetAllCurveloopsFromRevitFace(revitFace);
            var partitionedRevitCurveLoops = CurveLoopPartition.ByCurveLoops(revitCurveLoops);

            var listSurface = new List<Surface>();

            foreach (var curveloopPartition in partitionedRevitCurveLoops)
            {
                // convert the trimming curves
                var curveLoops = CurveLoopsAsPolyCurves(revitFace, curveloopPartition.GetAllCurves());

                // convert the underrlying surface
                var dyFace = (dynamic)revitFace;
                Surface untrimmedSrf = SurfaceExtractor.ExtractSurface(dyFace, curveLoops);
                if (untrimmedSrf == null)
                {
                    curveLoops.ForEach(x => x.Dispose());
                    curveLoops.Clear();
                    throw new Exception("Failed to extract surface");
                }

                // trim the surface
                Surface converted;
                try
                {
                    converted = untrimmedSrf.TrimWithEdgeLoops(curveLoops, 0.001);
                }
                catch (Exception e)
                {
                    curveLoops.ForEach(x => x.Dispose());
                    curveLoops.Clear();
                    untrimmedSrf.Dispose();
                    throw e;
                }

                curveLoops.ForEach(x => x.Dispose());
                curveLoops.Clear();
                untrimmedSrf.Dispose();

                // perform unit conversion if necessary
                if (performHostUnitConversion)
                    UnitConverter.ConvertToDynamoUnits(ref converted);

                // if possible, apply revit reference
                var revitRef = referenceOverride ?? revitFace.Reference;
                if (revitRef != null) converted = ElementFaceReference.AddTag(converted, revitRef);

                listSurface.Add(converted);
            }

            return listSurface;
        }

        private static List<PolyCurve> CurveLoopsAsPolyCurves(Face face,
            IEnumerable<IEnumerable<Autodesk.Revit.DB.Curve>> curveLoops)
        {
            List<PolyCurve> result = new List<PolyCurve>();
            foreach(var curveLoop in curveLoops)
            {
                List<Autodesk.DesignScript.Geometry.Curve> curves =
                    new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (var curve in curveLoop)
                {
                    curves.Add(curve.ToProtoType(false));
                }
                result.Add(PolyCurve.ByJoinedCurves(curves, 0.001, false));
                curves.ForEach(x => x.Dispose());
                curves.Clear();
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using CoEdge = Autodesk.DesignScript.Geometry.CoEdge;
using Edge = Autodesk.DesignScript.Geometry.Edge;
using Face = Autodesk.DesignScript.Geometry.Face;
using ProtoPt = Autodesk.DesignScript.Geometry.Point;
using Solid = Autodesk.DesignScript.Geometry.Solid;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.GeometryConversion
{
   [SupressImportIntoVM]
   public static class DynamoToRevitBRep
   {
       /// <summary>
       /// this method attempts to construct a BRep from a closed solid.
       /// </summary>
       /// <param name="sol"></param>
       /// <param name="performHostUnitConversion"></param>
       /// <returns></returns>
       public static GeometryObject ToRevitType(Solid sol,
          bool performHostUnitConversion = true)
       {
            var geometry = ToRevitType(sol, performHostUnitConversion, BRepType.Solid);

           if (geometry == null)
           {
               // An unexpected failure occurred when attempting to convert the solid into a Revit BRep.
               throw new Exception(Properties.Resources.DynamoSolidToRevitBRepFailure);
           }

            return geometry;
       }

       /// <summary>
       /// this method attempts to construct a BRep from a surface.
       /// </summary>
       /// <param name="surf"></param>
       /// <param name="performHostUnitConversion"></param>
       /// <returns></returns>
       public static GeometryObject ToRevitType(Surface surf,
          bool performHostUnitConversion = true)
       {
            var geometry = ToRevitType(surf, performHostUnitConversion, BRepType.OpenShell);

            if (geometry == null)
            {
                // An unexpected failure occurred when attempting to convert the surface into a Revit BRep
                throw new Exception(Properties.Resources.DynamoSurfaceToRevitBRepFailure);
            }

           return geometry;
       }

       private static GeometryObject ToRevitType(Autodesk.DesignScript.Geometry.Topology topology,
       bool performHostUnitConversion,
       BRepType type)
       {
           var faces = topology.Faces.ToList();
           var brb = new BRepBuilder(type);
           var edge2EdgeId = new Dictionary<Edge, BRepBuilderGeometryId>();

           //foreach face in solid/surface
           foreach (Face protoFace in faces)
           {
               using(var geom = protoFace.SurfaceGeometry())
               {
                   using(var ngeom = geom.ToNurbsSurface())
                   {
                       bool flipped = false;
                       // Check if the nurbs surface has flipped compared to the original surface
                       if (geom.NormalAtParameter(.5, .5).Dot(ngeom.NormalAtParameter(.5, .5)) < 0)
                       {
                           flipped = true;
                       }

                       // Create Revit nurbs surface
                       var bbface = BRepBuilderSurfaceGeometry.CreateNURBSSurface(ngeom.DegreeU, ngeom.DegreeV,
                           ngeom.UKnots(), ngeom.VKnots(), ngeom.ControlPoints().SelectMany(x => x.Select(y => y.ToXyz(performHostUnitConversion))).ToList(),
                           ngeom.Weights().SelectMany(x => x).ToList(),
                           false,
                           null);

                       // Add face
                       var faceId = brb.AddFace(bbface, flipped);

                       // add loops and connected edges
                       foreach (var loop in protoFace.Loops)
                       {
                           var loopId = brb.AddLoop(faceId);

                           foreach (var coedge in loop.CoEdges)
                           {
                               var edge = coedge.Edge;
                               BRepBuilderGeometryId edgeId;
                               if (edge2EdgeId.ContainsKey(edge))
                               {
                                   edgeId = edge2EdgeId[edge];
                               }
                               else
                               {
                                   var curve = edge.CurveGeometry;
                                   // Revit is already projecting edges onto the surface after checking for loop consistency
                                   // and is quite forgiving even when we use the nurbs surface instead of 
                                   // the original surface.
                                   //
                                   // But there are cases when edges ends up slightly outside one of the surfaces and Revit fails.
                                   //
                                   // This is something that we can be improve going forward.
                                   edgeId = brb.AddEdge(BRepBuilderEdgeGeometry.Create(curve.ToRevitType(performHostUnitConversion)));
                                   edge2EdgeId[edge] = edgeId;
                               }
                               brb.AddCoEdge(loopId, edgeId, coedge.Reversed);
                           }

                           brb.FinishLoop(loopId);
                       }

                       brb.FinishFace(faceId);
                   }
               }
           }

           //clean up everything
           edge2EdgeId.ToList().ForEach(x => x.Key.Dispose());
           faces.ForEach(x => x.Dispose());

           // Get result
           var outcome = brb.Finish();
           var converted = brb.GetResult();

           return converted;
       }
   }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.DesignScript.Runtime;
using ProtoPt = Autodesk.DesignScript.Geometry.Point;
using System.Diagnostics;

namespace Revit.GeometryConversion
{
   [SupressImportIntoVM]
   public static class ProtoToRevitSolid
   {
     /// <summary>
     /// this method attmempts to construct a BRep from a closed solid, it requires that the normals are oriented consistently.
     /// </summary>
     /// <param name="sol"></param>
     /// <param name="performHostUnitConversion"></param>
     /// <returns></returns>
       public static GeometryObject ToRevitType(Autodesk.DesignScript.Geometry.Solid sol,
          bool performHostUnitConversion = true)
       {
           //a new brep builder for solids
           var brb = new BRepBuilder(BRepType.Solid);

           Autodesk.Revit.DB.Solid converted = null;

           var edgeDict = new Dictionary<Autodesk.DesignScript.Geometry.Edge, BRepBuilderGeometryId>();
           if (performHostUnitConversion)
           {
               // Add check
               var newSol = sol.InHostUnits();
               var faces = newSol.Faces.ToList();


               //foreach face in solid
               foreach (Autodesk.DesignScript.Geometry.Face protoFace in faces)
               {
                   var geom = protoFace.SurfaceGeometry();

                   var ngeom = geom.ToNurbsSurface();
                   bool flipped = false;

                   if (geom.NormalAtParameter(.5, .5).Dot(ngeom.NormalAtParameter(.5, .5)) < 0)
                   {
                       flipped = true;
                   }


                   var bbface = BRepBuilderSurfaceGeometry.CreateNURBSSurface(ngeom.DegreeU, ngeom.DegreeV,
                       ngeom.UKnots(), ngeom.VKnots(), ngeom.ControlPoints().SelectMany(x => x.Select(y => y.ToXyz())).ToList(),
                       ngeom.Weights().SelectMany(x => x).ToList(),
                       false,
                       new BoundingBoxUV());

                   geom.Dispose();

                   var faceId = brb.AddFace(bbface, flipped);

                   foreach (var loop in protoFace.Loops)
                   {
                       var loopId = brb.AddLoop(faceId);

                       foreach (var coedge in loop.CoEdges)
                       {
                           var edge = coedge.Edge;
                           BRepBuilderGeometryId edgeId;
                           if (edgeDict.ContainsKey(edge))
                           {
                               edgeId = edgeDict[edge];
                           }
                           else
                           {
                               var curve = edge.CurveGeometry;

                               var projectedGeom = ngeom.ProjectInputOnto(curve, ngeom.NormalAtParameter(.5, .5).Reverse());
                               var newCurve = projectedGeom.First() as Autodesk.DesignScript.Geometry.Curve;

                               edgeId = brb.AddEdge(BRepBuilderEdgeGeometry.Create(edge.CurveGeometry.ToRevitType()));
                               edgeDict[edge] = edgeId;
                           }

                           brb.AddCoEdge(loopId, edgeId, coedge.Reversed);
                       }

                       brb.FinishLoop(loopId);
                   }

                   brb.FinishFace(faceId);
                   ngeom.Dispose();
               }
               edgeDict.ToList().ForEach(x => x.Key.Dispose());

               //clean up everything
               faces.ForEach(x => x.Dispose());
               newSol.Dispose();

               var outcome = brb.Finish();
               converted = brb.GetResult();
               
           }
           if (converted == null)
           {
               throw new Exception("An unexpected failure occurred when attempting to convert the solid");
           }

           return converted;
       }

       /// <summary>
       /// this method attmempts to construct a BRep from a surface
       /// </summary>
       /// <param name="surf"></param>
       /// <param name="performHostUnitConversion"></param>
       /// <returns></returns>
       public static GeometryObject ToRevitType(Autodesk.DesignScript.Geometry.Surface surf,
          bool performHostUnitConversion = true)
       {
           //a new brep builder for surfaces
           var brb = new BRepBuilder(BRepType.OpenShell);
           Autodesk.Revit.DB.Solid converted = null;

           var edgeDict = new Dictionary<Autodesk.DesignScript.Geometry.Edge, BRepBuilderGeometryId>();

           if (performHostUnitConversion)
           {
               var newSol = surf.InHostUnits();
               var faces = newSol.Faces.ToList();

               //foreach face in solid
               foreach (Autodesk.DesignScript.Geometry.Face protoFace in faces)
               {
                   var geom = protoFace.SurfaceGeometry();
                   var ngeom = surf.ToNurbsSurface();
                   //we need to perserve the intial curves before transforming to nurbsSurface because this conversion loses trimming loops sometimes... :(
                   bool flipped = false;

                   if(geom.NormalAtParameter(.5, .5).Dot(ngeom.NormalAtParameter(.5, .5)) < 0)
                   {
                       flipped = true;
                   }

                   var bbface = BRepBuilderSurfaceGeometry.CreateNURBSSurface(ngeom.DegreeU, ngeom.DegreeV,
                       ngeom.UKnots(), ngeom.VKnots(), ngeom.ControlPoints().SelectMany(x => x.Select(y => y.ToXyz())).ToList(),
                       ngeom.Weights().SelectMany(x => x).ToList(),
                       false,
                       new BoundingBoxUV());

                   geom.Dispose();
                   ngeom.Dispose();

                   var faceId = brb.AddFace(bbface, flipped);

                   foreach (var loop in protoFace.Loops)
                   {
                       var loopId = brb.AddLoop(faceId);

                       foreach (var coedge in loop.CoEdges)
                       {
                           var edge = coedge.Edge;
                           BRepBuilderGeometryId edgeId;
                           if (edgeDict.ContainsKey(edge))
                           {
                               edgeId = edgeDict[edge];
                           }
                           else
                           {
                               edgeId = brb.AddEdge(BRepBuilderEdgeGeometry.Create(edge.CurveGeometry.ToRevitType()));
                               edgeDict[edge] = edgeId;
                           }

                           brb.AddCoEdge(loopId, edgeId, coedge.Reversed);
                       }

                       brb.FinishLoop(loopId);
                   }

                   brb.FinishFace(faceId);
               }

               //clean up everything
               edgeDict.ToList().ForEach(x => x.Key.Dispose());
               faces.ForEach(x => x.Dispose());
               newSol.Dispose();

               var outcome = brb.Finish();
               converted = brb.GetResult();

           }
           if (converted == null)
           {
               throw new Exception("An unexpected failure occurred when attempting to convert the surface");
           }

           return converted;
       }
   }
}

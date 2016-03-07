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
using Unfold.Interfaces;
using System.Diagnostics;

namespace Revit.GeometryConversion
{
   [SupressImportIntoVM]
   public static class ProtoToRevitFace
    {
       /// <summary>
       /// This method converts a protogeometry face to a nurbsSurface for the BRepBuilder.
       /// </summary>
       /// <param name="protoFace">a protogeometry face to extract a surface from</param>
       /// <returns> a tuple of the surfacGeo and a bool representing if the converted face's normal is flipped relative to the orignal face, and a list of trimming edges</returns>
       public static Tuple<Autodesk.Revit.DB.BRepBuilderSurfaceGeometry,bool,List<Autodesk.DesignScript.Geometry.CoEdge>> ProtoFaceToBrepBuilderNurbsSurface(Autodesk.DesignScript.Geometry.Face protoFace)
       {
           var surf = protoFace.SurfaceGeometry();
           var nsurf = surf.ToNurbsSurface();
           //we need to perserve the intial curves before transforming to nurbsSurface because this conversion loses trimming loops sometimes... :(
           var edges = protoFace.Loops[0].CoEdges.ToList();

           bool flipped = false;
 
           if (!surf.NormalAtParameter(.5,.5).IsAlmostEqualTo(nsurf.NormalAtParameter(.5,.5)))
           {
               flipped = true;
           }


           var bbface = BRepBuilderSurfaceGeometry.CreateNURBSSurface(nsurf.DegreeU,nsurf.DegreeV,
               nsurf.UKnots(),nsurf.VKnots(),nsurf.ControlPoints().SelectMany(x=>x.Select(y=>y.ToXyz())).ToList(),
               nsurf.Weights().SelectMany(x=>x).ToList(),
               false,
               new BoundingBoxUV());
           surf.Dispose();
           nsurf.Dispose();
           return Tuple.Create(bbface, flipped, edges);
       }

       /// <summary>
       /// this method is a naive loop sorter from an unsorted list of curves
       /// </summary>
       /// <param name="curves"></param>
       /// <returns></returns>
       public static List<List<Autodesk.DesignScript.Geometry.Curve>> SortLoops(List<Autodesk.DesignScript.Geometry.Curve> curves)
       {
           var output = new List<List<Autodesk.DesignScript.Geometry.Curve>>();
           var curvesq = curves.ToList();
           while (curvesq.Count > 0)
           {
               var currentLoop = new List<Autodesk.DesignScript.Geometry.Curve>();
               currentLoop.Add(curvesq.First());
               var startpoint = currentLoop.Last().StartPoint;
               curvesq.RemoveAt(0);

               var currentCurve = currentLoop.Last();
               //by looping this many times we are sure that we'll compare every curve
               //could be much bettter...
               foreach (var crvv in curvesq)
               {
                   foreach (var crv in curvesq)
                   {
                       if ( (currentCurve.StartPoint.IsAlmostEqualTo(crv.StartPoint) || (currentCurve.StartPoint.IsAlmostEqualTo(crv.EndPoint))) ||
                           (currentCurve.EndPoint.IsAlmostEqualTo(crv.StartPoint) || (currentCurve.EndPoint.IsAlmostEqualTo(crv.EndPoint))))
                       {
                           //check its not the same curve.
                           var sameCurve = false;
                           foreach (var item in currentLoop)
                           {
                               
                               if (item.StartPoint.IsAlmostEqualTo(crv.StartPoint) && item.EndPoint.IsAlmostEqualTo(crv.EndPoint))
                               {
                                   sameCurve = true;
                                   break;
                               }
                              
                           }

                           if (sameCurve == true)
                           {
                               continue;
                           }

                           currentLoop.Add(crv);
                           currentCurve = currentLoop.Last();


                           
                             var test=   PolyCurve.ByJoinedCurves(currentLoop);
                             if (test.IsClosed)
                             {
                                 goto done;
                             }
                           

                       }
                   }
               }
           done:
               //remove everything we added to the current loop
               currentLoop.ForEach(x => curvesq.Remove(x));
               output.Add(currentLoop);
           }
           return output;
       }

    }

 public static class TopologyHelpers
 {
     /// <summary>
     /// this method analyzes normals for inside outside by checking the number of intersections a ray cast along the normal produces
     /// </summary>
     /// <param name="sol"></param>
     /// <returns></returns>
     public static List<Autodesk.DesignScript.Geometry.Surface> AnalyzeNormals2(Autodesk.DesignScript.Geometry.Solid sol)
     {
         var faces = sol.Faces;
         var surfs = faces.Select(x => x.SurfaceGeometry()).ToList();
         //dispose faces
         faces.ForEach(x => x.Dispose());
         var outsurfs = new List<Autodesk.DesignScript.Geometry.Surface>();
         var index = 0;
         foreach (var surf in surfs)
         {

             var norm = surf.NormalAtParameter(.5, .5);
             //TODO centroid/polygon or iterate until we find a point that actually lies on the surface...
             var bigline = Autodesk.DesignScript.Geometry.Line.ByStartPointDirectionLength(surf.PointAtParameter(.5, .5), norm, 1000);
             //intersect the list of surfaces except this surface with the line...
             var results = new List<Geometry>();

             foreach (var surfToIntersect in surfs.Except(new List<Autodesk.DesignScript.Geometry.Surface>() { surf }))
             {
                 results.AddRange(surfToIntersect.Intersect(bigline));
             }

             if (results.Count() % 2 == 0)
             {
                 //this ray had an even number of intersection so we do nothing
                 outsurfs.Add(surf.Translate(0, 0, 0) as Autodesk.DesignScript.Geometry.Surface);
             }

             else
             {

                 var flipped = surf.FlipNormalDirection();
                 //if areas are not equal than something went wrong...
                 if (Math.Abs(surf.Area - flipped.Area) > .001)
                 {
                     var trimcurve = PolyCurve.ByJoinedCurves(surf.PerimeterCurves());
                     var newflipped = flipped.TrimWithEdgeLoops(new List<PolyCurve>() { trimcurve });
                     flipped.Dispose();
                     flipped = newflipped;
                     trimcurve.Dispose();

                 }

                 //odd intersection, flip this surface and replace it in the list
                 outsurfs.Add(flipped);
             }
             index = index + 1;

             results.ForEach(x => x.Dispose());
             bigline.Dispose();
             norm.Dispose();
         }

         surfs.ForEach(x => x.Dispose());
         return outsurfs;

     }
 }


   [SupressImportIntoVM]
   public static class ProtoToRevitSolid
   {
       /// <summary>
       /// this method determines if the edge loop direction is ccw or cw, uses the invariant that face.verts are ccw order.(which does not seem true)
       /// </summary>
       /// <param name="loop"></param>
       /// <param name="srf"></param>
       /// <returns></returns>
       private static bool EdgeLoopDirectionIsClockwise2(List<Autodesk.DesignScript.Geometry.Curve> loop, Autodesk.DesignScript.Geometry.Face face)
       {
           var startIndex = 0;

           var verts = face.Vertices;
           Autodesk.DesignScript.Geometry.Point startPoint = null;
           Autodesk.DesignScript.Geometry.Curve startCurve = null;
           var clockwise = false;

           while (startCurve == null)
           {

              
               //scan the loop looking for the some vertex as a start vertex
               foreach (var crv in loop)
               {
                   if (crv.StartPoint.IsAlmostEqualTo(verts[startIndex].PointGeometry))
                   {
                       startPoint = verts[startIndex].PointGeometry;
                       startCurve = crv;
                       break;
                   }
               }
               if (startCurve != null)
               {
                   if (verts[startIndex].PointGeometry.IsAlmostEqualTo(startCurve.EndPoint))
                   {
                       //counter clockwise like the vert order

                   }
                   else
                   {
                       //nope clockwise
                       clockwise = true;
                   }
               }
                   //if we didnt find a matching curve... then try the next vert which is the start point of something else
               else
               {
                   startIndex++;
               }

           }



           verts.ForEach(x => x.Dispose());
           startCurve.Dispose();
           startPoint.Dispose();
           return clockwise;


       }


       /// <summary>
       /// this method extracts the coedges as pairs of verts from a face with a new version of LibG, then tries to match every curve in the loop to those coedges, if they are
       /// not all in the same direction as the coedges than we return false, otherwise true.
       /// </summary>
       /// <param name="loop"></param>
       /// <param name="face"></param>
       /// <returns></returns>
       private static bool LoopMatchesCoedgeDirections(List<Autodesk.DesignScript.Geometry.Curve> loop, Autodesk.DesignScript.Geometry.Face face)
       {
           var coedges = Unfold.Topology.Tesselation.MeshHelpers.Split<Autodesk.DesignScript.Geometry.Point>(face.Vertices.Select(x => x.PointGeometry).ToList(), 2);
          var totalMatchess = 0;
          foreach (var crv in loop)
          {
              var matchEdges = coedges.Where(x => x[0].IsAlmostEqualTo(crv.StartPoint) && x[1].IsAlmostEqualTo(crv.EndPoint)).ToList();
              if (matchEdges.Count()>0)
              {
                  totalMatchess = totalMatchess + 1;
              } 
          }

          if (totalMatchess == loop.Count)
          {
              //same loop as the coedge loop
              return true;
           
          }
           //not same loop
          return false;
       }

       /// <summary>
       /// this method determines if the edge loop direction is ccw or cw, not sure this works for concave edges...may need to tessellate those...
       /// </summary>
       /// <param name="loop"></param>
       /// <param name="face"></param>
       /// <returns></returns>
       private static bool EdgeLoopDirectionIsClockwise(List<Autodesk.DesignScript.Geometry.Curve> loop, Autodesk.DesignScript.Geometry.Surface face)
       {
           //calculate their cross product. If the cross product has a positive dot product with the normal you are going ccw, otherwise cw

           var firstedge = loop[0];
           var secondedge = loop[1];

           var firstXsecond = Vector.ByTwoPoints(firstedge.StartPoint, firstedge.EndPoint).Cross(Vector.ByTwoPoints(secondedge.StartPoint, secondedge.EndPoint));

           var crossDotNorm = firstXsecond.Dot(face.NormalAtParameter(.5, .5));

           if (crossDotNorm < 0)
           {
               return true;
           }
           return false;
       }


       


     /// <summary>
     /// this method attmempts to construct a BRep from a closed solid, it requires that the normals are oriented consistently.
     /// </summary>
     /// <param name="sol"></param>
     /// <param name="performHostUnitConversion"></param>
     /// <returns></returns>
       public static GeometryObject ToRevitType(Autodesk.DesignScript.Geometry.Solid sol,
          bool performHostUnitConversion = true)
       {

           //make the normals consistent for a solid
           //var consistentsol = AnalyzeNormals2(sol);

           //a new brep builder for solids
           var brb = new BRepBuilder(BRepType.Solid);

//           var addedEdges = new Dictionary<Unfold.Topology.EdgeLikeEntity, BRepBuilderGeometryId>(new Unfold.Interfaces.SpatialEqualityComparer<Unfold.Topology.EdgeLikeEntity>());
//          var EdgeIDtoEdgeDict = new Dictionary<BRepBuilderGeometryId,Autodesk.DesignScript.Geometry.Curve>();
//           var coEdgeDict = new Dictionary<Unfold.Topology.EdgeLikeEntity, List<Autodesk.DesignScript.Geometry.Curve>>(new Unfold.Interfaces.SpatialEqualityComparer<Unfold.Topology.EdgeLikeEntity>());


           //this data structures are just for debugging

//           var addedCoEdges = new List<List<Autodesk.DesignScript.Geometry.Curve>>();

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

                  /*
                  //convert the surface present in the face to a nurbsSurface and
                  //construct a nurbsSurface BrepBuilderGeo type from it
                var surfGeoData = ProtoToRevitFace.ProtoFaceToBrepBuilderNurbsSurface(protoFace);
                  //add that surface as a face into the BrepBuilder, use the boolean as the surface reversed flag
                var addeFace = brb.AddFace(surfGeoData.Item1,surfGeoData.Item2);
                  //add a loop representing the edges bounding the face
                var addedloop = brb.AddLoop(addeFace);
                var faceEdges = surfGeoData.Item3;
                  
                //check that there are the same number of faceEdges as curves in the surface...
                if (faceEdges.Count != protoFace.SurfaceGeometry().PerimeterCurves().Count())
                {
                    throw new Exception("edge conversion error - not the same number of edges or curves");
                }

                var curves = faceEdges.Select(x => x.CurveGeometry).ToList();
                var loop = PolyCurve.ByJoinedCurves(curves);
                
                //test the direction of the loop
                var dir = LoopMatchesCoedgeDirections(loop.Curves().ToList(), protoFace);
                var loopCurves = loop.Curves().ToList();
                  //if the direction is clockwise we must reverse the loop curves...
                if (dir == false)
                {
                    var curvecount = loopCurves.Count;
                    loopCurves = loopCurves.Select(x => x.Reverse()).ToList();
                    //and then reverse the list too...
                    loopCurves.Reverse();
                    if (curvecount != loopCurves.Count())
                    {
                        throw new Exception("somehow after reversal there are an incorrect number of curves in this loop");
                    }

                }


                var currentLoop = new List<Autodesk.DesignScript.Geometry.Curve>();
                addedCoEdges.Add(currentLoop);
                
               //foreach edge of the loopcurve that bounds the face.

                foreach (var edge  in loopCurves)
                {
                    //convert the edge to a revit curve
                    var curveGeo = edge;
                    var startPt = curveGeo.StartPoint;
                    var revCrv = curveGeo.ToRevitType();

                    //if the startpoint doesnt match the endpoint of the last added edge, then reverse the curve...
                    if (!curveGeo.StartPoint.IsAlmostEqualTo(revCrv.GetEndPoint(0).ToPoint()))
                    {
                        throw new Exception("revit curve is reversed for some reason");
                    }

                    //wrap the edge in an edge like, so we can compare it to other edges by startpoint endpoint more easily
                    var wrappedEdge = new Unfold.Topology.EdgeLikeEntity(curveGeo);
                    var reversed = false;


                    //dont add the edge if it's already been added from a previous FaceEdgeLoop, this means we'll only ever have 1 shared geometrical edge.
                    //but we'll still add the co-edge, though we need to reason about its direction
                    if (!addedEdges.ContainsKey(wrappedEdge))
                    {
                        var addedEdge = brb.AddEdge(BRepBuilderEdgeGeometry.Create(revCrv));

                        //store this added edge in the list of added edges
                        addedEdges.Add(wrappedEdge, addedEdge);
                        EdgeIDtoEdgeDict.Add(addedEdge, curveGeo);

                    }

                    //we're also building another data structure to keep track of the co-edges, not just the topo edges
                    //if we've never seen this edge before in the coedge dict, then add it
                    if (!coEdgeDict.ContainsKey(wrappedEdge))
                    {
                        coEdgeDict.Add(wrappedEdge, new List<Autodesk.DesignScript.Geometry.Curve>());
                        coEdgeDict[wrappedEdge].Add(curveGeo);
                     }

                        //if we have seen this coedge before, then we need to add the other coedge... which should be filpped.
                    else
                    {
                            reversed = true;
                            coEdgeDict[wrappedEdge].Add(curveGeo.Reverse());
                    }
                    
                       
                        var edgeId = addedEdges[wrappedEdge];

                        //check how many coedges are here... it should never be greater than 1 if we are about to add another
                        brb.AddCoEdge(addedloop, edgeId, reversed);
                        currentLoop.Add(coEdgeDict[wrappedEdge].Last());
                        if (coEdgeDict[wrappedEdge].Count > 2)
                        {
                            throw new Exception("too many coedge entries");
                        }

                   
                }
                brb.FinishLoop(addedloop);
                brb.FinishFace(addeFace);
              }
 */ 
               //clean up everything
               //clean up everything
               faces.ForEach(x => x.Dispose());
               newSol.Dispose();
//               addedEdges.ToList().ForEach(x => x.Key.Dispose());
//               coEdgeDict.ToList().ForEach(x => x.Key.Dispose());
//               addedCoEdges.SelectMany(x => x).ToList().ForEach(x=>x.Dispose());

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
//           var addedEdges = new Dictionary<Unfold.Topology.EdgeLikeEntity, BRepBuilderGeometryId>(new Unfold.Interfaces.SpatialEqualityComparer<Unfold.Topology.EdgeLikeEntity>());
//           var EdgeIDtoEdgeDict = new Dictionary<BRepBuilderGeometryId, Autodesk.DesignScript.Geometry.Curve>();
//           var coEdgeDict = new Dictionary<Unfold.Topology.EdgeLikeEntity, List<Autodesk.DesignScript.Geometry.Curve>>(new Unfold.Interfaces.SpatialEqualityComparer<Unfold.Topology.EdgeLikeEntity>());
//           var addedCoEdges = new List<List<Autodesk.DesignScript.Geometry.Curve>>();
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
//                   if (!geom.NormalAtParameter(.5, .5).IsAlmostEqualTo(ngeom.NormalAtParameter(.5, .5)))
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

                   
                   /*
                   //convert the surface present in the face to a nurbsSurface and
                   //construct a nurbsSurface BrepBuilderGeo type from it
                   var surfGeoData = ProtoToRevitFace.ProtoFaceToBrepBuilderNurbsSurface(protoFace);
                   //add that surface as a face into the BrepBuilder
                   var addeFace = brb.AddFace(surfGeoData.Item1, surfGeoData.Item2);
                  
                   var faceEdges = surfGeoData.Item3;

                   //check that there are the same number of faceEdges as curves in the surface...
                   if (faceEdges.Count != protoFace.SurfaceGeometry().PerimeterCurves().Count())
                   {
                       throw new Exception("edge conversion error - not the same number of edges or curves");
                   }

                   var curves = faceEdges.Select(x => x.CurveGeometry).ToList();

                   //get the number of loops
                  var listlistcurves = ProtoToRevitFace.SortLoops(curves);

                  var loops = listlistcurves.Select(x => PolyCurve.ByJoinedCurves(x)).ToList();
                   //for now a hack...
                  loops.Sort((loop1, loop2) => loop1.Length.CompareTo(loop2.Length));
                  loops.Reverse();

                  foreach (var loop in loops)
                  {

                      //add a loop representing the edges bounding the face
                      var addedloop = brb.AddLoop(addeFace);

                      //TODO we need to make outer loops CCW, innter loops CW
                      // we'll probably need to test polygon containement of loops against each other this will let us pick the correct direction
                      //to check for.... all would be trivial if supplied coedge points from libG in lists...

                      //test the direction of the loop
                      var dir = LoopMatchesCoedgeDirections(loop.Curves().ToList(), protoFace);
                       var loopCurves = loop.Curves().ToList();
                      //if the direction is clockwise we must reverse the loop curves...
                       if ((dir == false ))
                      {
                          var curvecount = loopCurves.Count;
                          loopCurves = loopCurves.Select(x => x.Reverse()).ToList();
                          //and then reverse the list too...
                          loopCurves.Reverse();
                          if (curvecount != loopCurves.Count())
                          {
                              throw new Exception("somehow after reversal there are an incorrect number of curves in this loop");
                          }

                      }


                      var currentLoop = new List<Autodesk.DesignScript.Geometry.Curve>();
                      addedCoEdges.Add(currentLoop);

                      //foreach edge of the loopcurve that bounds the face.

                      foreach (var edge in loopCurves)
                      {
                          //convert the edge to a revit curve
                          var curveGeo = edge;
                          var startPt = curveGeo.StartPoint;
                          var revCrv = curveGeo.ToRevitType();

                          //if the startpoint doesnt match the endpoint of the last added edge, then reverse the curve...
                          if (!curveGeo.StartPoint.IsAlmostEqualTo(revCrv.GetEndPoint(0).ToPoint()))
                          {
                              throw new Exception("revit curve is reversed for some reason");
                          }

                          //wrap the edge in an edge like, so we can compare it to other edges...
                          var wrappedEdge = new Unfold.Topology.EdgeLikeEntity(curveGeo);
                          var reversed = false;


                          //dont add the edge if it's already been added from a previous FaceEdgeLoop, this means we'll only ever have 1 shared edge.
                          //but we'll still add the co-edge, though we need to reason about its direction
                          if (!addedEdges.ContainsKey(wrappedEdge))
                          {
                              var addedEdge = brb.AddEdge(BRepBuilderEdgeGeometry.Create(revCrv));

                              //store this added edge in the list of added edges
                              addedEdges.Add(wrappedEdge, addedEdge);
                              EdgeIDtoEdgeDict.Add(addedEdge, curveGeo);

                          }

                          //we're also building another data structure to keep track of the co-edges, not just the topo edges
                          //if we've never seen this edge before in the coedge dict, then add it
                          if (!coEdgeDict.ContainsKey(wrappedEdge))
                          {
                              coEdgeDict.Add(wrappedEdge, new List<Autodesk.DesignScript.Geometry.Curve>());
                              coEdgeDict[wrappedEdge].Add(curveGeo);
                          }

                              //if we have seen this coedge before, then we need to add the other coedge... which should be filpped.
                          else
                          {
                              reversed = true;
                              coEdgeDict[wrappedEdge].Add(curveGeo.Reverse());
                          }


                          var edgeId = addedEdges[wrappedEdge];

                          //check how many coedges are here... it should never be greater than 1 if we are about to add another
                          brb.AddCoEdge(addedloop, edgeId, reversed);
                          currentLoop.Add(coEdgeDict[wrappedEdge].Last());
                          if (coEdgeDict[wrappedEdge].Count > 2)
                          {
                              throw new Exception("too many coedge entries");
                          }


                      }
                      brb.FinishLoop(addedloop);
                  }
                   brb.FinishFace(addeFace);
                    */
               }
               //clean up everything
               edgeDict.ToList().ForEach(x => x.Key.Dispose());
               faces.ForEach(x => x.Dispose());
               newSol.Dispose();
//               addedEdges.ToList().ForEach(x => x.Key.Dispose());
//               coEdgeDict.ToList().ForEach(x => x.Key.Dispose());
//               addedCoEdges.SelectMany(x => x).ToList().ForEach(x => x.Dispose());

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Dynamo;
using Dynamo.Visualization;


namespace Unfold.Topology
{
    public class Tesselation
    {
       public class MeshHelpers
        {
            public static List<List<T>> Split<T>(List<T> source, int subListLength)
            {
                return source.
                   Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => x.Index / subListLength)
                   .Select(x => x.Select(v => v.Value).ToList())
                   .ToList();
            }

            public static Point SurfaceAsPolygonCenter(Surface surface){
                
                var oldgeo = new List<DesignScriptEntity>();
                List<Point> ptlist = new List<Point>();
                foreach (var curve in surface.PerimeterCurves())
                {
                    ptlist.Add(curve.StartPoint);
                    ptlist.Add(curve.EndPoint);
                    oldgeo.Add(curve);
                }
                var centroid = Vector.ByCoordinates(0, 0, 0);
                foreach (var pt in ptlist)
                {
                    centroid = pt.AsVector().Add(centroid);
                }

                centroid = centroid.Scale(1.0 / ptlist.Count);
                //cleanup all the perimeter curves we extracted to do this calculation
                foreach (IDisposable item in oldgeo)
                {
                    item.Dispose();
                }
                return surface.ClosestPointTo(centroid.AsPoint());
            
            }
           
            public static Point SurfaceAsPolygonCenter(List<Surface> surfaces)
            {
                var oldgeo = new List<DesignScriptEntity>();
                List<Point> ptlist = new List<Point>();
                foreach (var surface in surfaces)
                {//TODO cleanup these curves
                    foreach (var curve in surface.PerimeterCurves())
                    {
                        ptlist.Add(curve.StartPoint);
                        ptlist.Add(curve.EndPoint);
                        oldgeo.Add(curve);
                    }
                }
                var centroid = Vector.ByCoordinates(0, 0, 0);
               foreach(var pt in ptlist)
               {
                   centroid = pt.AsVector().Add(centroid);
               }

               centroid = centroid.Scale(1.0 / ptlist.Count);
               var points =  surfaces.Select(x=>x.ClosestPointTo(centroid.AsPoint())).ToList();

               double mindist = 100000000000;
               Surface closeSurf = null;
               for (int i = 0; i < surfaces.Count; i++)
               {
                   var distance = Vector.ByTwoPoints(points[i],centroid.AsPoint()).Length;
                   if (distance < mindist)
                   {
                       closeSurf = surfaces[i];
                       mindist = distance;
                   }
               }

                //cleanup all the perimeter curves we extracted to do this calculation
               foreach (IDisposable item in oldgeo)
               {
                   item.Dispose();
               }

               return closeSurf.ClosestPointTo(centroid.AsPoint());

            }



        }
        public static List<List<Point>> Tessellate(List<Face> faces,double tolerance,int maxGridLines)
        {
            var surfaces = faces.Select(x => x.SurfaceGeometry()).ToList();

            var result = Tessellate(surfaces,tolerance,maxGridLines);
            return result;

        }


        public static List<List<Point>> Tessellate(List<Surface> surfaces,double tolerance,int maxGridLines)
        {
            var rpfactory = new DefaultRenderPackageFactory();
            var rp = rpfactory.CreateRenderPackage();
           var tp = new TessellationParameters();
            tp.MaxTessellationDivisions = maxGridLines;
            tp.Tolerance = tolerance;
            
            foreach (var surface in surfaces)
            {
                surface.Tessellate(rp, tp);
                

            }
            //grab double components from rp and subset them into points and further into triangles
            List<List<double>> pointdata = MeshHelpers.Split(rp.MeshVertices.ToList(), 3);

            List<Point> points = pointdata.Select(x => Point.ByCoordinates(x[0], x[1], x[2])).ToList();

            List<List<Point>> tris = MeshHelpers.Split(points, 3);

            return tris;



        }

    }
}

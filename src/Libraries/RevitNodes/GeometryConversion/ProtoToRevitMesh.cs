using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Dynamo;

using RevitServices.Materials;
using Autodesk.DesignScript.Geometry;

namespace Revit.GeometryConversion
{
    [IsVisibleInDynamoLibrary(false)]
    [SupressImportIntoVM]
    public static class ProtoToRevitMesh
    {

        public static IList<GeometryObject> ToRevitType(this Autodesk.DesignScript.Geometry.Surface srf,
            TessellatedShapeBuilderTarget target = TessellatedShapeBuilderTarget.Mesh,
            TessellatedShapeBuilderFallback fallback = TessellatedShapeBuilderFallback.Salvage,
            ElementId MaterialId = null,
            bool performHostUnitConversion = true)
        {
            var rp = new DefaultRenderPackage();
            if (performHostUnitConversion)
            {
                var newSrf = srf.InHostUnits();
                newSrf.Tessellate(rp, new TessellationParameters());
                newSrf.Dispose();
            }
            else
            {
                srf.Tessellate(rp, new TessellationParameters());
            }

            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            var v = rp.MeshVertices.ToList();

            for (int i = 0; i < v.Count; i += 9)
            {
                var a = new XYZ(v[i], v[i + 1], v[i + 2]);
                var b = new XYZ(v[i + 3], v[i + 4], v[i + 5]);
                var c = new XYZ(v[i + 6], v[i + 7], v[i + 8]);

                var face = new TessellatedFace(new List<XYZ>() { a, b, c },  MaterialId != null ? MaterialId : MaterialsManager.Instance.DynamoMaterialId);
                tsb.AddFace(face);
            }

            tsb.CloseConnectedFaceSet();

            var result = tsb.Build(target, fallback, ElementId.InvalidElementId);
            return result.GetGeometricalObjects();
        }

        public static IList<GeometryObject> ToRevitType(
            this Autodesk.DesignScript.Geometry.Solid solid,
             TessellatedShapeBuilderTarget target = TessellatedShapeBuilderTarget.Mesh,
            TessellatedShapeBuilderFallback fallback = TessellatedShapeBuilderFallback.Salvage,
             ElementId MaterialId = null,
            bool performHostUnitConversion = true)
        {
            var rp = new DefaultRenderPackage();
            if (performHostUnitConversion)
            {
                var newSolid = solid.InHostUnits();
                newSolid.Tessellate(rp, new TessellationParameters());
                newSolid.Dispose();
            }
            else
            {
                solid.Tessellate(rp, new TessellationParameters());
            }

            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            var v = rp.MeshVertices.ToList();

            for (int i = 0; i < v.Count; i += 9)
            {
                var a = new XYZ(v[i], v[i + 1], v[i + 2]);
                var b = new XYZ(v[i + 3], v[i + 4], v[i + 5]);
                var c = new XYZ(v[i + 6], v[i + 7], v[i + 8]);

                var face = new TessellatedFace(new List<XYZ>() { a, b, c }, MaterialId != null ? MaterialId : MaterialsManager.Instance.DynamoMaterialId);
                tsb.AddFace(face);
            }

            tsb.CloseConnectedFaceSet();
            var result = tsb.Build(target, fallback, ElementId.InvalidElementId);
            return result.GetGeometricalObjects();
        }
       
        //this method checks intersections between non contigous edges of a quad for self intersection
        private static bool QuadSelfIntersects(List<Autodesk.DesignScript.Geometry.Point> quadVerts)
        {
            var AB = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(quadVerts[0], quadVerts[1]);
            var BC = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(quadVerts[1], quadVerts[2]);
            var CD = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(quadVerts[2], quadVerts[3]);
            var DA = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(quadVerts[3], quadVerts[0]);

            if (AB.DoesIntersect(CD))
            {
                return true;
            }

            if (BC.DoesIntersect(DA))
            {
                return true;
            }
            return false;
        }

        public static IList<GeometryObject> ToRevitType(
            this Autodesk.DesignScript.Geometry.Mesh mesh,
             TessellatedShapeBuilderTarget target = TessellatedShapeBuilderTarget.Mesh,
            TessellatedShapeBuilderFallback fallback = TessellatedShapeBuilderFallback.Salvage,
             ElementId MaterialId = null,
            bool performHostUnitConversion = true)
        {
         
            var verts = mesh.VertexPositions;
            var indicies = mesh.FaceIndices.ToList();

            var currentVerts = new List<Autodesk.DesignScript.Geometry.Point>();
            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            for(int faceindex = 0; faceindex<indicies.Count();faceindex++)
            {
                var f = indicies[faceindex];
                currentVerts.Clear();
                var currentIndicies = new List<uint>() { f.A, f.B, f.C, f.D };
                for (int i = 0; i < f.Count; i++)
                {
                    var currentindex = Convert.ToInt32(currentIndicies[i]);

                    currentVerts.Add(verts[currentindex]);
                }
                if (f.Count > 3)
                {
                    //test if the face is a planar...
                    var BACNormal = Vector.ByTwoPoints(currentVerts[1], currentVerts[0]).Cross(Vector.ByTwoPoints(currentVerts[1], currentVerts[2])).Normalized();
                    var DACNormal = Vector.ByTwoPoints(currentVerts[3], currentVerts[0]).Cross(Vector.ByTwoPoints(currentVerts[3], currentVerts[2])).Normalized();

                    //if the two triangle normals are not parallel then we have a non planar quad
                    //and we'll skip adding this face and instead add two new faces to list to process
                    //these new faces are two triangles representing the quad
                    if (Math.Abs(BACNormal.Dot(DACNormal) - 1) > 0.000001 ||
                        //or if there are any self intersections between non 
                        //contigous polygon edges we also triangulate, this finds a twisted quad
                        QuadSelfIntersects(currentVerts)
                        )
                    {
                        indicies.Add(IndexGroup.ByIndices(f.B, f.C, f.A));
                        indicies.Add(IndexGroup.ByIndices(f.A, f.C, f.D));
                        continue;
                    }
                }
              
                //convert all the points to Revit XYZ vectors and perform unit conversion here
                var xyzs = currentVerts.Select(x => x.ToXyz(performHostUnitConversion)).ToList();

                var face = new TessellatedFace(xyzs, MaterialId != null ? MaterialId :MaterialsManager.Instance.DynamoMaterialId );
                tsb.AddFace(face);
            }

            tsb.CloseConnectedFaceSet();
          
            var result = tsb.Build(target, fallback, ElementId.InvalidElementId);

            foreach (IDisposable vert in verts)
            {
                vert.Dispose();
            }

            return result.GetGeometricalObjects();
        }
    }
}

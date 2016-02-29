using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Materials;
using Autodesk.DesignScript.Geometry;
using Dynamo.Visualization;

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

                var face = new TessellatedFace(new List<XYZ>() { a, b, c }, MaterialId != null ? MaterialId : MaterialsManager.Instance.DynamoMaterialId);
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

        public static IList<GeometryObject> ToRevitType(
            this Autodesk.DesignScript.Geometry.Mesh mesh,
             TessellatedShapeBuilderTarget target = TessellatedShapeBuilderTarget.Mesh,
            TessellatedShapeBuilderFallback fallback = TessellatedShapeBuilderFallback.Salvage,
             ElementId MaterialId = null,
            bool performHostUnitConversion = true)
        {

            var verts = mesh.VertexPositions;
            var indices = mesh.FaceIndices;

            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            for (int faceindex = 0, count = indices.Count(); faceindex < count; faceindex++)
            {
                var f = indices[faceindex];
                //if this is a quad face triangulate it
                if (f.Count > 3)
                {
                    //and add two triangles to the tessellated shape builder
                    var tri1 = IndexGroup.ByIndices(f.B, f.C, f.A);
                    var tri2 = IndexGroup.ByIndices(f.A, f.C, f.D);

                    AddFace(tsb, tri1, verts, performHostUnitConversion, MaterialId);
                    AddFace(tsb, tri2, verts, performHostUnitConversion, MaterialId);
                }
                else
                {
                    AddFace(tsb, f, verts, performHostUnitConversion, MaterialId);
                }
            }

            tsb.CloseConnectedFaceSet();

            var result = tsb.Build(target, fallback, ElementId.InvalidElementId);

            foreach (IDisposable vert in verts)
            {
                vert.Dispose();
            }

            return result.GetGeometricalObjects();
        }
        
        /// <summary>
        /// This is to create a bounding box mesh for geometries which have errors during the tessellating process
        /// </summary>
        /// <param name="minPoint"></param>
        /// <param name="maxPoint"></param>
        /// <param name="performHostUnitConversion"></param>
        /// <returns></returns>
        public static IList<GeometryObject> CreateBoundingBoxMeshForErrors(Autodesk.DesignScript.Geometry.Point minPoint,
            Autodesk.DesignScript.Geometry.Point maxPoint, bool performHostUnitConversion = true)
        {
            double x0 = minPoint.X;
            double y0 = minPoint.Y;
            double z0 = minPoint.Z;
            double x1 = maxPoint.X;
            double y1 = maxPoint.Y;
            double z1 = maxPoint.Z;

            x0 -= 1.0;
            y0 -= 1.0;
            z0 -= 1.0;
            x1 += 1.0;
            y1 += 1.0;
            z1 += 1.0;

            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            var p0 = new XYZ(x0, y0, z0);
            var p1 = new XYZ(x1, y0, z0);
            var p2 = new XYZ(x1, y1, z0);
            var p3 = new XYZ(x0, y1, z0);
            var p4 = new XYZ(x0, y0, z1);
            var p5 = new XYZ(x1, y0, z1);
            var p6 = new XYZ(x1, y1, z1);
            var p7 = new XYZ(x0, y1, z1);

            var f1 = new TessellatedFace(new List<XYZ>() { p0, p1, p2 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f1);
            var f2 = new TessellatedFace(new List<XYZ>() { p2, p3, p0 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f2);
            var f3 = new TessellatedFace(new List<XYZ>() { p0, p1, p5 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f3);
            var f4 = new TessellatedFace(new List<XYZ>() { p5, p4, p0 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f4);
            var f5 = new TessellatedFace(new List<XYZ>() { p1, p2, p6 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f5);
            var f6 = new TessellatedFace(new List<XYZ>() { p6, p5, p1 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f6);
            var f7 = new TessellatedFace(new List<XYZ>() { p2, p3, p7 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f7);
            var f8 = new TessellatedFace(new List<XYZ>() { p7, p6, p2 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f8);
            var f9 = new TessellatedFace(new List<XYZ>() { p3, p0, p4 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f9);
            var f10 = new TessellatedFace(new List<XYZ>() { p4, p7, p3 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f10);
            var f11 = new TessellatedFace(new List<XYZ>() { p4, p5, p6 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f11);
            var f12 = new TessellatedFace(new List<XYZ>() { p6, p7, p4 }, MaterialsManager.Instance.DynamoErrorMaterialId);
            tsb.AddFace(f12);

            tsb.CloseConnectedFaceSet();
            var result = tsb.Build(TessellatedShapeBuilderTarget.Mesh, TessellatedShapeBuilderFallback.Salvage, 
                ElementId.InvalidElementId);
            return result.GetGeometricalObjects();
        }

        /// <summary>
        /// this method converts a ProtoGeometry IndexGroup and Points to a Revit tessellated face, and adds it
        //  to the TessellatedShape Builder that is passed in.
        /// </summary>
        /// <param name="tsb">a Revit TessellatedShapeBuilder which we wish to add a face to </param>
        /// <param name="f"> a ProtoGeometry indexGroup defining a Mesh face</param>
        /// <param name="meshVerts">a vertex array of Points which should be the 
        /// entire vertex array extracted from a ProtoGeometry Mesh.</param>
        /// <param name="performHostUnitConversion">a Bool which enables host unit conversion scaling</param>
        /// <param name="materialId">an ElementId represnting the Material we want to apply to this face</param>
        private static void AddFace(TessellatedShapeBuilder tsb,
            IndexGroup f,
            Autodesk.DesignScript.Geometry.Point[] meshVerts,
            bool performHostUnitConversion,
            ElementId materialId)
        {
            var xyzs = new List<Autodesk.Revit.DB.XYZ>();
            var currentIndices = new List<uint>() { f.A, f.B, f.C, f.D };

            for (int i = 0; i < f.Count; i++)
            {
                var currentindex = Convert.ToInt32(currentIndices[i]);
                //convert all the points to Revit XYZ vectors and perform unit conversion here
                xyzs.Add(meshVerts[currentindex].ToXyz(performHostUnitConversion));
            }

            var face = new TessellatedFace(xyzs, materialId != null ? materialId : MaterialsManager.Instance.DynamoMaterialId);
            tsb.AddFace(face);
        }
    }
}

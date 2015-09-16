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

            for (int faceindex = 0; faceindex < indices.Count(); faceindex++)
            {
                var f = indices[faceindex];
                //if this is a quad face triangulate it
                if (f.Count > 3)
                {
                    //and add two triangles to the tessellated shape builder
                    var tri1 = IndexGroup.ByIndices(f.B, f.C, f.A);
                    var tri2 = IndexGroup.ByIndices(f.A, f.C, f.D);

                    AddProtoFaceToTSB(tsb, tri1, verts, performHostUnitConversion, MaterialId);
                    AddProtoFaceToTSB(tsb, tri2, verts, performHostUnitConversion, MaterialId);
                }
                else
                {
                    AddProtoFaceToTSB(tsb, f, verts, performHostUnitConversion, MaterialId);
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

        //this method converts a proto indexGroup and verts to a tessellated face, and adds it
        //to the tessellated shape builder that is passed in.
        //the verts array should be the entire vert array extracted from a proto mesh.
        private static void AddProtoFaceToTSB(TessellatedShapeBuilder tsb,
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

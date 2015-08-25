using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Dynamo;
using Dynamo.DSEngine;

using RevitServices.Materials;
using RevitServices.Persistence;

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

            var result = tsb.Build(target, fallback, ElementId.InvalidElementId).GetBuildResult();
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
            var result = tsb.Build(target, fallback, ElementId.InvalidElementId).GetBuildResult();
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
            var indicies = mesh.FaceIndices;

            if (performHostUnitConversion)
            {
                var newverts = verts.Select(x => x.InHostUnits()).ToArray();
                foreach (IDisposable point in verts)
                {
                    point.Dispose();
                }
                verts = newverts;
                Array.Clear(newverts, 0, newverts.Length);
            }

            var currentVerts = new List<Autodesk.DesignScript.Geometry.Point>();
            var tsb = new TessellatedShapeBuilder();
            tsb.OpenConnectedFaceSet(false);

            foreach (var f in indicies)
            {

                currentVerts.Clear();
                var currentIndicies = new List<uint>() { f.A, f.B, f.C, f.D };
                for (int i = 0; i < f.Count; i++)
                {
                    var currentindex = Convert.ToInt32(currentIndicies[i]);

                    currentVerts.Add(verts[currentindex]);
                }

                //convert all the points to Revit XYZ vectors
                var xyzs = currentVerts.Select(x => x.ToXyz()).ToList();

                var face = new TessellatedFace(xyzs, MaterialId != null ? MaterialId :MaterialsManager.Instance.DynamoMaterialId );
                tsb.AddFace(face);
            }

            tsb.CloseConnectedFaceSet();
            var result = tsb.Build(target, fallback,  ElementId.InvalidElementId).GetBuildResult();

            foreach (IDisposable vert in verts)
            {
                vert.Dispose();
            }

            return result.GetGeometricalObjects();
        }
    }
}

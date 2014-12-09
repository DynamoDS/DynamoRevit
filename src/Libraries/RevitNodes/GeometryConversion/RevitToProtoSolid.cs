using System.Collections.Generic;

using Autodesk.DesignScript.Runtime;

using Face = Autodesk.Revit.DB.Face;
using Solid = Autodesk.DesignScript.Geometry.Solid;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.GeometryConversion
{
    [SupressImportIntoVM]
    public static class RevitToProtoSolid
    {
        public static Autodesk.DesignScript.Geometry.Solid ToProtoType(this Autodesk.Revit.DB.Solid solid, 
            bool performHostUnitConversion = true)
        {
            var faces = solid.Faces;
            var srfs = new List<Surface>();
            foreach (Face face in faces)
            {
                srfs.AddRange(face.ToProtoType(false));
            }
            var converted = Solid.ByJoinedSurfaces(srfs);
            srfs.ForEach(x => x.Dispose());
            srfs.Clear();

            if (performHostUnitConversion)
                UnitConverter.ConvertToDynamoUnits(ref converted);

            return converted;
        }
    }
}

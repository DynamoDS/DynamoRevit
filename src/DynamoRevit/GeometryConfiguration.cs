using System;
using Dynamo.Interfaces;

namespace Dynamo.Applications
{
    class GeometryConfiguration : IGeometryConfiguration
    {
        public bool PreloadShapeManager
        {
            get { return false; } // ASM is always preloaded in Revit.
        }

        public string ShapeManagerPath
        {
            // Returning 'false' for 'PreloadShapeManager' so this property
            // will not be called, throw an exception if it's been called.
            get { throw new NotImplementedException(); }
        }

        public LibraryVersion Version
        {
            // This is specific to current Revit version.
            get { return LibraryVersion.Version219; }
        }
    }
}

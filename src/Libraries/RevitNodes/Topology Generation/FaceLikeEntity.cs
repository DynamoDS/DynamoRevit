using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Unfold.Interfaces;
using System.Collections;

namespace Unfold.Topology
{
    [SupressImportIntoVM]
    /// <summary>
    /// This is a wrapper type for face like entities so that unfolding methods can operate both on faces/edges or surfaces/adjacent-curves
    /// There are overloads for building this class from faces or surfaces
    /// </summary>
    public class FaceLikeEntity : IUnfoldablePlanarFace<EdgeLikeEntity>, IDisposable
    {

        public Object OriginalEntity { get; set; }

        private List<Surface> _surface;
        public List<Surface> SurfaceEntities
        {
            get { return _surface; }
            set
            {
                _surface = value;
                EdgeLikeEntities = ExtractSurfaceEdges(_surface);
            }
        }
        public List<EdgeLikeEntity> EdgeLikeEntities { get; set; }
        public int ID { get; set; }
        public List<int> IDS { get; set; }


        private List<EdgeLikeEntity> ExtractSurfaceEdges(List<Surface> surfaces)
        {

            var pericurves = surfaces.SelectMany(x => x.PerimeterCurves()).ToList();
            var ees = pericurves.Select(y => new EdgeLikeEntity(y)).ToList();


            return ees;
            // return surfaces.SelectMany(x => x.PerimeterCurves()).Select(y=> new EdgeLikeEntity(y)).ToList();

        }


        public FaceLikeEntity(Surface surface)
        {

            //store the surface
            SurfaceEntities = new List<Surface>() { surface };
            //store surface
            OriginalEntity = surface;
            // new blank ids
            IDS = new List<int>();

        }

        public FaceLikeEntity(Face face)
        {
            //grab the surface from the face
            SurfaceEntities = new List<Surface> { face.SurfaceGeometry() };
            // org entity is the face
            OriginalEntity = face;
            // grab edges
            // List<Edge> orgedges = face.Edges.ToList();
            //wrap edges
            // EdgeLikeEntities = orgedges.ConvertAll(x => new EdgeLikeEntity(x));
            // new blank ids list

            IDS = new List<int>();


        }

        public FaceLikeEntity()
        {
            IDS = new List<int>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (OriginalEntity != null)
                {
#if DEBUG
                    Console.WriteLine("disposing orginal entity");
#endif
                    if (OriginalEntity is IEnumerable)
                        foreach (IDisposable item in (IEnumerable)OriginalEntity)
                        {
                            item.Dispose();
                        }
                    else
                    {
                        ((IDisposable)OriginalEntity).Dispose();
                    }
                }

                if (SurfaceEntities != null)
                {
                    foreach (IDisposable item in SurfaceEntities)
                    {
#if DEBUG
                        Console.WriteLine("disposing surface entity");
#endif
                        item.Dispose();
                    }
                }
                foreach (IDisposable item in EdgeLikeEntities)
                {
#if DEBUG
                    Console.WriteLine("disposing edgelike contained in facelike");
#endif
                    item.Dispose();
                }

            }


        }

    }
}

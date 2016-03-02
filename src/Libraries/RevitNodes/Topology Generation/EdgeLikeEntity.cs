using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Unfold.Interfaces;

namespace Unfold.Topology
{
    [SupressImportIntoVM]
    /// <summary>
    /// wrapper for edges and curves
    /// </summary>
    public class EdgeLikeEntity : IUnfoldableEdge, IDisposable
    {

        public Point Start { get; set; }
        public Point End { get; set; }
        public Curve Curve { get; set; }
        public Edge RealEdge { get; set; }

        public EdgeLikeEntity(Edge edge)
        {
            Start = edge.StartVertex.PointGeometry;
            End = edge.EndVertex.PointGeometry;
            Curve = edge.CurveGeometry;
            RealEdge = edge;

        }

        public EdgeLikeEntity(Curve curve)
        {
            Start = curve.StartPoint;
            End = curve.EndPoint;
            Curve = curve;
            RealEdge = null;


        }

        public int GetSpatialHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                //implementing XOR hashcode since a curve defined edge should hash to the same value even if its
                // start and end points are stored in the reverse fields.  Is there something better?

                int hash = this.Start.ToString().GetHashCode() ^ this.End.ToString().GetHashCode();
                // Console.WriteLine(this);
                // Console.WriteLine(hash);
                return hash;

            }
        }

        public bool SpatialEquals(ISpatiallyEquatable obj)
        {
            IUnfoldableEdge objitem = obj as IUnfoldableEdge;
            var otherval = objitem.Start.ToString() + objitem.End.ToString();

            //equals will return true even if the start and end point are reversed since on one object this should be
            // the same edge, if this is equal is not implemented this way surface perimeter curves fail to be located correctly 
            // in hash tables

            if (otherval == this.Start.ToString() + this.End.ToString() || otherval == this.End.ToString() + this.Start.ToString())
            {
                return true;
            }
            else
            {

                return false;
            }
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
                ((IDisposable)Curve).Dispose();
                if (RealEdge != null)
                {
                    ((IDisposable)RealEdge).Dispose();
                }
                ((IDisposable)Start).Dispose();
                ((IDisposable)End).Dispose();

            }

        }
    }
}

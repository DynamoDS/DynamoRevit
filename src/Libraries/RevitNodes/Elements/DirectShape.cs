using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;
using DynamoUnits;
using Revit.Elements.InternalUtilities;

using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;
using Autodesk.DesignScript.Geometry;
using System.Collections.Generic;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit DirectShape, which is a wrapper for some other geometrical entities 
    /// </summary>
    [RegisterForTrace]
    public class DirectShape:Element
    {
         #region Internal Properties

        internal Autodesk.Revit.DB.DirectShape InternalDirectShape
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalDirectShape; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Wrap an existing DirectShape.
        /// </summary>
        /// <param name="shape"></param>
        protected DirectShape(Autodesk.Revit.DB.DirectShape shape)
        {
            SafeInit(() => InitDirectShape(shape));
        }

       /// <summary>
       /// Internal Constructor for an existing DirectShape.
       /// </summary>
       /// <param name="geos"></param>
       /// <param name="shapename"></param>
       /// <param name="cat"></param>
        protected DirectShape(IList<Autodesk.Revit.DB.GeometryObject> geos, string shapename,ElementId cat)
        {
            SafeInit(() => InitDirectShape(geos, shapename, cat));
        }


        /// <summary>
        /// Initialize a DirectShape element
        /// </summary>
        /// <param name="shape"></param>
        private void InitDirectShape(Autodesk.Revit.DB.DirectShape shape)
        {
            InteralSetDirectShape(shape);
        }

        /// <summary>
        /// Initialize a DirectShape element
        /// </summary>
        private void InitDirectShape(IList<Autodesk.Revit.DB.GeometryObject> geos,
            string shapeName,
            ElementId categoryID)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldShape =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.DirectShape>(Document);

            //There was a oldDirectShape, rebind to that
            if (oldShape != null)
            {
                InteralSetDirectShape(oldShape);
                return;
            }

            //Phase 2- There was no existing shape, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.DirectShape ds;

            if (Document.IsFamilyDocument)
            {
                //TODO (MIKE) is this possible?
                ds = NewDirectShape(geos,
                    Document, categoryID,
                    Guid.NewGuid().ToString(), shapeName);

            }
            else
            {
                ds = NewDirectShape(geos,
                    Document, categoryID,
                    Guid.NewGuid().ToString(), shapeName);
            }

            InteralSetDirectShape(ds);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// SetInternalElement to a DirectShape element
        /// </summary>
        /// <param name="shape"></param>
        private void InteralSetDirectShape(Autodesk.Revit.DB.DirectShape shape)
        {
            this.InternalDirectShape = shape;
            this.InternalElementId = shape.Id;
            this.InternalUniqueId = shape.UniqueId;
        }

      
        private static IList<Autodesk.Revit.DB.GeometryObject> Tessellate(Surface surf, int graphicsStyle, string name)
        {
             return surf.ToRevitType(TessellatedShapeBuilderTarget.Solid,TessellatedShapeBuilderFallback.Mesh);
        }

        private static IList<Autodesk.Revit.DB.GeometryObject> Tessellate(Autodesk.DesignScript.Geometry.Solid solid, int graphicsStyle, string name)
        {
            return solid.ToRevitType(TessellatedShapeBuilderTarget.Solid, TessellatedShapeBuilderFallback.Mesh);
        }

        //This method forked from: //https://github.com/jeremytammik/DirectObjLoader
        /// <summary>
        /// Create a new DirectShape element from given list of XYZ's and indicies
        /// </summary>
        private static Autodesk.Revit.DB.DirectShape NewDirectShape(
         IList<Autodesk.Revit.DB.GeometryObject> geos,
          Document doc,
            ElementId categoryId,
          string appGuid,
          string shapeName)
        {
           
            var ds = Autodesk.Revit.DB.DirectShape.CreateElement(
              doc, categoryId, appGuid, shapeName);

            ds.SetShape(geos);
            ds.Name = shapeName;
           
            return ds;
        }



       

        #endregion

        #region Private mutators

        #endregion

        #region Public properties

        
      
        /// <summary>
        /// Gets the location of the specific DirectShape 
        /// </summary>
        public Point Location
        {
            get
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                DocumentManager.Regenerate();
                var pos = InternalDirectShape.Location as LocationPoint;
                TransactionManager.Instance.TransactionTaskDone();
                return pos.Point.ToPoint();
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit DirectShape given some geometry, and name for the shape, and a category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="name"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        public static DirectShape BySurfaceNameCategory(Surface surface, string name, Category cat)
        {
            if (surface == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (cat == null)
            {
                throw new ArgumentNullException("category");
            }

            return new DirectShape(surface.ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh), name, new ElementId(cat.Id));
        }

        public static DirectShape BySolidNameCategory(Autodesk.DesignScript.Geometry.Solid solid, string name, Category cat)
        {
            if (solid == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (cat == null)
            {
                throw new ArgumentNullException("category");
            }

            return new DirectShape(solid.ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh), name, new ElementId(cat.Id));
        }

        public static DirectShape ByMeshNameCategory(Autodesk.DesignScript.Geometry.Mesh mesh, string name, Category cat)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (cat == null)
            {
                throw new ArgumentNullException("category");
            }
            
            return new DirectShape(mesh.ToRevitType(TessellatedShapeBuilderTarget.Mesh, TessellatedShapeBuilderFallback.Salvage), name, new ElementId(cat.Id));
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Construct a DirectShape from the Revit document. 
        /// </summary>
        /// <param name="directShape"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static DirectShape FromExisting(Autodesk.Revit.DB.DirectShape directShape, bool isRevitOwned)
        {
            return new DirectShape(directShape)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return InternalDirectShape.Name;
        }

      
        
    }
}

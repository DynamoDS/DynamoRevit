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
    public class DirectShape : Element
    {
        #region Internal Properties

        internal Autodesk.Revit.DB.DirectShape InternalDirectShape
        {
            get;
            private set;
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
        ///  Internal Constructor for a new DirectShape
        /// </summary>
        /// <param name="shapeReference"></param>
        /// <param name="tessellatedShape"></param>
        /// <param name="shapename"></param>
        /// <param name="cat"></param>
        protected DirectShape(DesignScriptEntity shapeReference ,IList<Autodesk.Revit.DB.GeometryObject> tessellatedShape, string shapename, ElementId cat)
        {
            SafeInit(() => InitDirectShape(shapeReference,tessellatedShape, shapename, cat));
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
        private void InitDirectShape(DesignScriptEntity shapeReference,
            IList<Autodesk.Revit.DB.GeometryObject> tessellatedShape,
            string shapeName,
            ElementId categoryID)
        {

            //Phase 1 - Check to see if the object exists and should be rebound
            var oldShape =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.DirectShape>(Document);

            //There was a oldDirectShape, rebind to that
            if (oldShape != null)
            {
                //set the directshape element
                this.InteralSetDirectShape(oldShape);

                //if the cateogryID has changed, we cannot continue to rebind and instead
                //will make a new directShape
                if (categoryID == this.InternalElement.Category.Id)
                {
                    //set the shape geometry of the directshape, this method passes in the actual input geo
                    //and checks if the elementID exists in the Tags dictionary on that geo - 
                    //this check is used to determine if the geometry should be updated 
                    this.InteralSetShape(shapeReference, tessellatedShape);
                    this.InteralSetName(shapeName);
                    return;
                }

            }
           
            //Phase 2- There was no existing shape, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.DirectShape ds;
            //actually construct the directshape revit element
            ds = NewDirectShape(tessellatedShape,
                Document, categoryID,
                Guid.NewGuid().ToString(), shapeName);

            InteralSetDirectShape(ds);
            //add the elementID to the tags dictionary on the real protogeometry input
            shapeReference.Tags.AddTag(this.InternalElementId.ToString(), true);
            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);

        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Create a new DirectShape element from given list of Revit GeometryObjects, document, category ID data
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

        /// <summary>
        /// Sets the internalDirectShape to point to some geometry
        /// </summary>
        /// <param name="shapeReference"></param>
        /// <param name="tessellatedShape"></param>
        private void InteralSetShape(DesignScriptEntity shapeReference, IList<Autodesk.Revit.DB.GeometryObject> tessellatedShape)
        {
            if (shapeReference.Tags.LookupTag(this.InternalElementId.ToString()) != null)
            {
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            this.InternalDirectShape.SetShape(tessellatedShape);
            shapeReference.Tags.AddTag(this.InternalElementId.ToString(), true);
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Sets the internalDirectShape to have a new name
        /// </summary>
        /// <param name="name"></param>
        private void InteralSetName(string name)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            if (name != this.InternalDirectShape.Name)
            {
                this.InternalDirectShape.SetName(name);
            }

            TransactionManager.Instance.TransactionTaskDone();
        }
        private void SetMaterials(IList<GeometryObject> geos , Material material)
        {   
            foreach (GeometryObject o in geos)
            {
                if (o is Autodesk.Revit.DB.Solid)
                {
                    Autodesk.Revit.DB.Solid solid = o as Autodesk.Revit.DB.Solid;
                    foreach (Autodesk.Revit.DB.Face face in solid.Faces)
                    {
                       Document.Paint(this.InternalDirectShape.Id,face ,new ElementId(material.Id));
                         
                    }
                }
            }
        }
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
        /// Create a Revit DirectShape given some geometry, a name for the shape, and a category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="surface">a surface that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape BySurfaceNameCategory(Surface surface, string name, Category category)
        {
            if (surface == null)
            {
                throw new ArgumentNullException("geometry");
            }
            
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            
            return new DirectShape(surface,surface.ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh), name, new ElementId(category.Id));
        }

        // <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, and a category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="solid">a solid that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape BySolidNameCategory(Autodesk.DesignScript.Geometry.Solid solid, string name, Category category,Material material)
        {
            if (solid == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            return new DirectShape(solid, solid.ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh,new ElementId(material.Id)), name, new ElementId(category.Id));
        }

        // <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, and a category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="mesh">a mesh that will be placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape ByMeshNameCategory(Autodesk.DesignScript.Geometry.Mesh mesh, string name, Category category)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
           
            return new DirectShape(mesh,mesh.ToRevitType(TessellatedShapeBuilderTarget.Mesh, TessellatedShapeBuilderFallback.Salvage), name, new ElementId(category.Id));
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

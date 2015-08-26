using System;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;

using Point = Autodesk.DesignScript.Geometry.Point;
using Autodesk.DesignScript.Geometry;
using System.Collections.Generic;
using RevitServices.Materials;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit DirectShape, which is a wrapper for some other geometrical entities 
    /// </summary>
    [RegisterForTrace]
    public class DirectShape : Element
    {
        
        #region Private Properties

        private static Guid DYNAMO_DIRECTSHAPE_APP_GUID
        {
            get
            {
                return Guid.Parse("03deaabe-6989-4d20-ba68-557001d2f45c");
            }

        }
        #endregion 

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
        /// <param name="material"></param>
        /// <param name="shapename"></param>
        /// <param name="category"></param>
        protected DirectShape(DesignScriptEntity shapeReference ,string shapename, ElementId category, ElementId material)
        {
            if (shapeReference == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (shapename == null)
            {
                throw new ArgumentNullException("name");
            }

            if (category == null)
            {
                throw new ArgumentNullException("category");
            }

            if (material == null)
            {
                throw new ArgumentNullException("material");
            }

            SafeInit(() => InitDirectShape(shapeReference, shapename, category, material));
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
            string shapeName,
            ElementId categoryId,
            ElementId materialId)
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
                if (categoryId == this.InternalElement.Category.Id)
                {
                    //set the shape geometry of the directshape, this method passes in the actual input geo
                    //and checks if the elementID exists in the Tags dictionary on that geo - 
                    //this check is used to determine if the geometry should be updated,
                    //we also check the material, if it's different than the currently assigned material
                    //then we need to rebuild the geo so that a new material is applied
                    //
                    this.InteralSetShape(shapeReference,materialId);
                    this.InteralSetName(shapeName);
                    return;
                }

            }
           
            //Phase 2- There was no existing shape, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.DirectShape ds;

            //generate the geometry correctly depending on the type of geo
            var tessellatedShape = GenerateTessellatedGeo(shapeReference, materialId);

            //actually construct the directshape revit element
            ds = NewDirectShape(tessellatedShape,
                Document, categoryId,
                DYNAMO_DIRECTSHAPE_APP_GUID.ToString(), shapeName);

            InteralSetDirectShape(ds);
            //add the elementID to the tags dictionary on the real protogeometry input
            shapeReference.Tags.AddTag(this.InternalElementId.ToString(), materialId);
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


        private static IList<GeometryObject> GenerateTessellatedGeo(DesignScriptEntity shapeReference, ElementId materialId)
        {
            IList<GeometryObject> tessellatedShape = null;
            if (shapeReference is Autodesk.DesignScript.Geometry.Mesh)
            {
                tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Mesh).ToRevitType(TessellatedShapeBuilderTarget.Mesh, TessellatedShapeBuilderFallback.Salvage, MaterialId: materialId);
            }
            else if (shapeReference is Autodesk.DesignScript.Geometry.Surface)
            {
                tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Surface).ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh, MaterialId: materialId);
            }

            else if (shapeReference is Autodesk.DesignScript.Geometry.Solid)
            {
                tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Solid).ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh, MaterialId: materialId);
            }

            if (tessellatedShape == null)
            {
                throw new ArgumentException("can only create DirectShapes from Solids, Surfaces or Meshes");
            }
            return tessellatedShape;
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
        /// Sets the internalDirectShape to point to some geometry,
        /// this method also generates tessellated geometry from the protogeometry object
        /// and sets the material of the generated Revit faces
        /// </summary>
        /// <param name="shapeReference"></param>
        /// <param name="materialId"></param>
        private void InteralSetShape(DesignScriptEntity shapeReference, ElementId materialId)
        {
            //if the elementID for the current directShape revitElement exists on the input Geometry AND
            //the value stored at that key is equal to the materialId we're trying to set then we don't
            //need to regenerate the geometry
            if((shapeReference.Tags.LookupTag(this.InternalElementId.ToString()) as ElementId) == materialId)
            {
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(Document);

            IList<GeometryObject> tessellatedShape = GenerateTessellatedGeo(shapeReference, materialId);

            this.InternalDirectShape.SetShape(tessellatedShape);
            shapeReference.Tags.AddTag(this.InternalElementId.ToString(), materialId);
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Sets the internalDirectShape to have a new name
        /// </summary>
        /// <param name="name"></param>
        private void InteralSetName(string name)
        {
          
            if (name != this.InternalDirectShape.Name)
            {
                TransactionManager.Instance.EnsureInTransaction(Document);
                this.InternalDirectShape.SetName(name);
                TransactionManager.Instance.TransactionTaskDone();
            }

            
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets the centroid of the geometry contained in DirectShape 
        /// </summary>
        public Point Centroid
        {
            get
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                DocumentManager.Regenerate();
                var Revitbb = this.InternalElement.get_BoundingBox(null);
                TransactionManager.Instance.TransactionTaskDone();
                var bb = Revitbb.ToProtoType();
                var cube = bb.ToCuboid();
                var point = cube.Centroid();
                bb.Dispose();
                cube.Dispose(); 
              
                return point;
            }
        }

        #endregion

        #region Public static constructors


        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, and category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="geometry">a surface that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape ByGeometryNameCategory(Autodesk.DesignScript.Geometry.Geometry geometry, string name, Category category)
        {
            if (geometry is Autodesk.DesignScript.Geometry.Solid || geometry is Autodesk.DesignScript.Geometry.Surface)
            {
                return new DirectShape(geometry, name, new ElementId(category.Id), MaterialsManager.Instance.DynamoMaterialId);
            }

            throw new ArgumentException("can only create DirectShapes from Solids, Surfaces or Meshes");
        }

        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, and category.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="mesh">a surface that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape ByMeshNameCategory(Autodesk.DesignScript.Geometry.Mesh mesh, string name, Category category)
        {
            return new DirectShape(mesh, name, new ElementId(category.Id), MaterialsManager.Instance.DynamoMaterialId);
        }


        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, category, and material.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="geometry">a solid that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <param name="material">a material to apply to the faces of the DirectShape</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape ByGeometryNameCategoryMaterial(Autodesk.DesignScript.Geometry.Geometry geometry, string name, Category category,Material material)
        {

            if (geometry is Autodesk.DesignScript.Geometry.Solid || geometry is Autodesk.DesignScript.Geometry.Surface)
            {
                return new DirectShape(geometry, name, new ElementId(category.Id), new ElementId(material.Id));
            }

            throw new ArgumentException("can only create DirectShapes from Solids, Surfaces or Meshes");
        }

        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, category, and material.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after placing a DirectShape, so
        /// a new DirectShape will be generated if the category input is changed
        /// </summary>
        /// <param name="mesh">a mesh that will be placed in the Revit model as a DirectShape</param>
        /// <param name="name">a string name for the directshape</param>
        /// <param name="category">must be a top level built-in category</param>
        /// <param name="material">a material to apply to the faces of the DirectShape</param>
        /// <returns>a DirectShape Element</returns>
        public static DirectShape ByMeshNameCategoryMaterial(Autodesk.DesignScript.Geometry.Mesh mesh, string name, Category category, Material material)
        {
            return new DirectShape(mesh, name, new ElementId(category.Id), new ElementId(material.Id));
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

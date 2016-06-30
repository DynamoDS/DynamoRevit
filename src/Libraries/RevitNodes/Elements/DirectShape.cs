using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Materials;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Revit.Elements
{
    /// <summary>
    /// This class acts as a representation of a directShape state, we can store it in trace
    /// and on protogeometry types (in their tags dictionary) to keep track of the state of
    /// a DirectShape wrapper type, it inherits from SerializeableId so that ElementLifeCycle
    /// and DocumentEvents continue to function for DirectShapes.
    /// </summary>
    [SupressImportIntoVM]
    [Serializable]
    public class DirectShapeState : SerializableId
    {
        public string syncId { get; set; }
        public int materialId { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("syncId", syncId, typeof(string));
            info.AddValue("materialId", materialId, typeof(int));
        }

        public DirectShapeState(DirectShape ds, string syncId, ElementId materialId) :
            base()
        {
            this.IntID = ds.InternalElement.Id.IntegerValue;
            this.StringID = ds.UniqueId;
            this.syncId = syncId;
            this.materialId = materialId.IntegerValue;
        }

        public DirectShapeState(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            syncId = (string)info.GetValue("syncId", typeof(string));
            materialId = (int)info.GetValue("materialId", typeof(int));
        }
    }

    /// <summary>
    /// A Revit DirectShape, which is a wrapper for some other geometrical entities 
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class DirectShape : Element
    {
        private const string DEFAULT_NAME = "DirectShape";

        #region Static Fields

        private static Guid DYNAMO_DIRECTSHAPE_APP_GUID;

        #endregion

        #region Public Static Properties

        [IsVisibleInDynamoLibrary(false)]
        public static Material DynamoPreviewMaterial { get; private set; }

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
        protected DirectShape(DesignScriptEntity shapeReference, string shapename, Category category, Material material)
        {
            SafeInit(() => InitDirectShape(shapeReference, shapename, category, material));
        }

        /// <summary>
        /// Initialize a DirectShape element
        /// </summary>
        private void InitDirectShape(Autodesk.Revit.DB.DirectShape shape)
        {
            InternalSetDirectShape(shape);
        }

        /// <summary>
        /// Initialize a DirectShape element
        /// </summary>
        private void InitDirectShape(DesignScriptEntity shapeReference,
            string shapeName,
            Category category,
            Material material)
        {
            //Phase 1 - Check to see if a DirectShape exists in trace and should be rebound
            var oldShape =
                ElementBinder.GetElementAndTraceData<Autodesk.Revit.DB.DirectShape, DirectShapeState>(Document);

            //There was a oldDirectShape, rebind to that
            if (oldShape != null)
            {
                //set the directshape element
                this.InternalSetDirectShape(oldShape.Item1);

                //if the cateogryID has changed, we cannot continue to rebind and instead
                //will make a new directShape
                if (category.Id == this.InternalElement.Category.Id.IntegerValue)
                {
                    //set the shape geometry of the directshape, this method passes in the actual input geo
                    //and checks the directShapeState object at the elementId key in the input geo tags dictionary. 
                    //this check is used to determine if the geometry should be updated, this is done by checking
                    //the sync guid in the DirectShapeState (a guid that is generated when geometry changes on
                    //the revit element.
                    //we also check the material, if it's different than the currently assigned material
                    //then we need to rebuild the geo so that a new material is applied
                    //
                    this.InternalSetShape(shapeReference, new ElementId(material.Id), oldShape.Item2.syncId);
                    this.InternalSetName(shapeReference, shapeName, material, category);
                    return;
                }
            }

            //Phase 2- There was no existing shape, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.DirectShape ds;

            //generate the geometry correctly depending on the type of geo
            var tessellatedShape = GenerateTessellatedGeo(shapeReference, new ElementId(material.Id));

            //actually construct the directshape revit element
            ds = NewDirectShape(tessellatedShape,
                Document, new ElementId(category.Id),
                DirectShape.DYNAMO_DIRECTSHAPE_APP_GUID.ToString(), shapeName);

            InternalSetDirectShape(ds);
            InternalSetName(shapeReference, shapeName, material, category);
            //generate a new syncId for trace
            var traceData = new DirectShapeState(this, Guid.NewGuid().ToString(), new ElementId(material.Id));

            //add the elementID:tracedata to the tags dictionary on the real protogeometry input
            shapeReference.Tags.AddTag(this.InternalElementId.ToString(), traceData);
            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetRawDataForTrace(traceData);

        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Create a new DirectShape element from given list of Revit GeometryObjects, document, category, and Iddata
        /// </summary>
        private static Autodesk.Revit.DB.DirectShape NewDirectShape(
         IList<Autodesk.Revit.DB.GeometryObject> geos,
          Document doc,
          ElementId categoryId,
          string appGuid,
          string shapeName)
        {
            var ds = Autodesk.Revit.DB.DirectShape.CreateElement(
              doc, categoryId, appGuid, Guid.NewGuid().ToString());

            ds.SetShape(geos);
            ds.Name = shapeName;

            return ds;
        }

        private static IList<GeometryObject> GenerateTessellatedGeo(DesignScriptEntity shapeReference, ElementId materialId)
        {
            IList<GeometryObject> tessellatedShape = null;
            if (shapeReference is Autodesk.DesignScript.Geometry.Mesh)
            {
                tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Mesh).ToRevitType(
                    TessellatedShapeBuilderTarget.Mesh,
                    TessellatedShapeBuilderFallback.Salvage,
                    MaterialId: materialId);
            }
            else if (shapeReference is Autodesk.DesignScript.Geometry.Surface)
            {
                // Defer to tessellated shape if BRep shape for some reason fails
                GeometryObject geometry = null;
                try
                {
                    geometry = DynamoToRevitBRep.ToRevitType(shapeReference as Autodesk.DesignScript.Geometry.Surface, true, materialId);
                }
                catch
                {

                }

                if (geometry != null)
                {
                    tessellatedShape = new List<GeometryObject>() { geometry };
                }
                else
                {
                    tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Surface).ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh, MaterialId: materialId);
                }
            }
            else if (shapeReference is Autodesk.DesignScript.Geometry.Solid)
            {
                // Defer to tessellated shape if BRep shape for some reason fails
                GeometryObject geometry = null;
                try
                {
                    geometry = DynamoToRevitBRep.ToRevitType(shapeReference as Autodesk.DesignScript.Geometry.Solid, true, materialId);
                }
                catch
                {

                }

                if (geometry != null)
                {
                    tessellatedShape = new List<GeometryObject>() { geometry };
                }
                else
                {
                    tessellatedShape = (shapeReference as Autodesk.DesignScript.Geometry.Solid).ToRevitType(TessellatedShapeBuilderTarget.AnyGeometry, TessellatedShapeBuilderFallback.Mesh, MaterialId: materialId);
                }
            }

            if (tessellatedShape == null)
            {
                throw new ArgumentException(Revit.Properties.Resources.DirectShapeInvalidArgument);
            }
            return tessellatedShape;
        }

        #endregion

        #region Private mutators
        /// <summary>
        /// SetInternalElement to a DirectShape element
        /// </summary>
        private void InternalSetDirectShape(Autodesk.Revit.DB.DirectShape shape)
        {
            this.InternalDirectShape = shape;
            this.InternalElementId = shape.Id;
            this.InternalUniqueId = shape.UniqueId;
        }

        /// <summary>
        /// Sets the internalDirectShape's shape to point to some geometry,
        /// this method also generates tessellated geometry from the protogeometry object
        /// and sets the material of the generated Revit faces
        /// </summary>
        private void InternalSetShape(DesignScriptEntity shapeReference, ElementId materialId, string currentSyncId)
        {
            //if the elementID for the current directShape revitElement exists on the input Geometry AND
            //the value stored at that key is equal to the materialId we're trying to set AND
            //the previousState's syncId (in the tags dictionary) equals the current syncId (from trace)
            //then we do not need to regenerate

            //first lookup the state on the input geometry
            var previousState = shapeReference.Tags.LookupTag(this.InternalElementId.ToString()) as DirectShapeState;
            //then compare values
            if (previousState != null && previousState.materialId == materialId.IntegerValue && previousState.syncId == currentSyncId)
            {
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(Document);

            var tessellatedShape = GenerateTessellatedGeo(shapeReference, materialId);

            this.InternalDirectShape.SetShape(tessellatedShape);

            //update the value in trace, since we had to modify the geometry we need a new syncId
            var updatedTraceData = new DirectShapeState(this, Guid.NewGuid().ToString(), materialId);
            ElementBinder.SetRawDataForTrace(updatedTraceData);
            //place new values in the tags dict
            if (shapeReference.Tags.LookupTag(this.InternalElementId.ToString()) == null)
            {
                shapeReference.Tags.AddTag(this.InternalElementId.ToString(), updatedTraceData);
            }
            else
            {
                var storedState = shapeReference.Tags.LookupTag(this.InternalElementId.ToString()) as DirectShapeState;
                storedState.syncId = updatedTraceData.syncId;
                storedState.materialId = updatedTraceData.materialId;
                storedState.IntID = updatedTraceData.IntID;
                storedState.StringID = updatedTraceData.StringID;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Sets the internalDirectShape to have a new name
        /// if this method detects the default string it generates a more specific name
        /// </summary>
        private void InternalSetName(DesignScriptEntity shapeReference, string name, Material material, Category category)
        {
            if (name == DEFAULT_NAME)
            {

                name = string.Format("DirectShape" + "(Geometry = {0}, Category = {1}, Material ={2}.)",
                    shapeReference.ToString(),
                    category != null ? category.Name : "",
                    material != null ? material.Name : "");
            }

            if (name != this.InternalDirectShape.Name)
            {
                TransactionManager.Instance.EnsureInTransaction(Document);
                this.InternalDirectShape.SetName(name);
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, a Category, and Material.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after creation, so
        /// a new DirectShape will be generated if the category input is changed.
        /// </summary>
        /// <param name="geometry">A Solid or Surface that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">A string name for the DirectShape</param>
        /// <param name="category">Must be a top level Built-in Category</param>
        /// <param name="material">A Material to apply to the faces of the DirectShape</param>
        /// <returns>A DirectShape Element</returns>
        public static DirectShape ByGeometry(Autodesk.DesignScript.Geometry.Geometry geometry,
            Category category,
            [DefaultArgumentAttribute(" DirectShape.DynamoPreviewMaterial")]Material material,
            string name = DEFAULT_NAME)
        {
            if (geometry == null)
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

            if (material == null)
            {
                throw new ArgumentNullException("material");
            }

            if (geometry is Autodesk.DesignScript.Geometry.Solid || geometry is Autodesk.DesignScript.Geometry.Surface)
            {
                return new DirectShape(geometry, name, category, material);
            }

            throw new ArgumentException(Revit.Properties.Resources.DirectShapeInvalidArgument);
        }

        /// <summary>
        /// Create a Revit DirectShape given some geometry, a name for the shape, a Category, and Material.
        /// The geometry will be tessellated before being placed in the Revit model
        /// The category of a DirectShape cannot be changed after creation, so
        /// a new DirectShape will be generated if the category input is changed.
        /// </summary>
        /// <param name="mesh">A Mesh that will be tessellated and placed in the Revit model as a DirectShape</param>
        /// <param name="name">A string name for the DirectShape</param>
        /// <param name="category">Must be a top level Built-in Category</param>
        /// <param name="material">A Material to apply to the faces of the DirectShape</param>
        /// <returns>A DirectShape Element</returns>
        public static DirectShape ByMesh(Autodesk.DesignScript.Geometry.Mesh mesh,
            Category category,
            [DefaultArgumentAttribute(" DirectShape.DynamoPreviewMaterial")]Material material,
           string name = DEFAULT_NAME)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            if (name == null)
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

            return new DirectShape(mesh, name, category, material);
        }

        #endregion

        #region static constructors

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

        static DirectShape()
        {
            DynamoPreviewMaterial = Material.ByName(Document.GetElement(MaterialsManager.Instance.DynamoMaterialId).Name);
            DYNAMO_DIRECTSHAPE_APP_GUID = Guid.Parse("03deaabe-6989-4d20-ba68-557001d2f45c");
        }

        #endregion

        /// <summary>
        /// Please see InternalSetName method
        /// </summary>
        public override string ToString()
        {
            return InternalDirectShape.IsValidObject ? InternalDirectShape.Name : String.Empty;
        }
    }
}

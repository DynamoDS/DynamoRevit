using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Color = DSCore.Color;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.Elements
{
    /// <summary>
    /// Superclass of all Revit element wrappers
    /// </summary>
    //[SupressImportIntoVM]
    public abstract class Element : IDisposable, IGraphicItem, IFormattable
    {
        /// <summary>
        /// Handling exceptions when calling the initializing function
        /// </summary>
        /// <param name="init"></param>
        protected void SafeInit(Action init)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            SafeInitImpl(init);
        }

        /// <summary>
        /// Handling exceptions when calling the initializing function
        /// </summary>
        /// <param name="init"></param>
        /// <param name="fromExistingElement"></param>
        protected void SafeInit(Action init, bool fromExistingElement)
        {
            if (!fromExistingElement)
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            SafeInitImpl(init);
        }

        private void SafeInitImpl(Action init)
        {
            var elementManager = ElementIDLifecycleManager<long>.GetInstance();
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Element>(Document);
            int count = 0;
            long id = 0;
            if (null != element)
            {
                id = element.Id.Value;
                count = elementManager.GetRegisteredCount(id);
            }
            try
            {
                init();
            }
            catch (Exception e)
            {
                //If the element is newly created and bound but the creation is aborted because
                //of an exception, it need to be unregistered.
                if (element == null && InternalElementId != null)
                {
                    elementManager.UnRegisterAssociation(Id, this);
                    internalId = null;
                    throw e;
                }
                else if (element != null)
                {
                    //If the internal element has already been bound, and if the registered count has increased,
                    //it need to be unregistered.
                    if (elementManager.GetRegisteredCount(id) == count + 1)
                    {
                        elementManager.UnRegisterAssociation(Id, this);
                        internalId = null;
                    }

                    //It means that the updating operation failed, an attemption of making a new element is made.
                    ElementBinder.SetRawDataForTrace(null);
                    SafeInit(init);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// A reference to the current Document.
        /// </summary>
        internal static Document Document
        {
            get { return DocumentManager.Instance.CurrentDBDocument; }
        }

        /// <summary>
        /// Indicates whether the element is owned by Revit or not.  If the element
        /// is Revit owned, it should not be deleted by Dispose().
        /// </summary>
        internal bool IsRevitOwned = false;

        /// <summary>
        /// Obtain all of the Parameters from an Element, sorted by Name.
        /// </summary>
        public Parameter[] Parameters
        {
            get
            {
                return
                    InternalElement.Parameters.Cast<Autodesk.Revit.DB.Parameter>()
                        .OrderBy(x => x.Definition.Name)
                        .Select(x => new Parameter(x))
                        .ToArray();
            }
        }

        /// <summary>
        /// Get the Name of the Element
        /// </summary>
        public string Name
        {
            get
            {
                return InternalElement.Name;
            }
        }

        /// <summary>
        /// Get an Axis-aligned BoundingBox of the Element
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                TransactionManager.Instance.EnsureInTransaction(Document);
                DocumentManager.Regenerate();
                var bb = InternalElement.get_BoundingBox(null);
                if (bb == null) 
                {
                    bb = InternalElement.get_BoundingBox(Document.GetElement(InternalElement.OwnerViewId) as Autodesk.Revit.DB.View);
                }
                TransactionManager.Instance.TransactionTaskDone();
                return bb.ToProtoType();
            }
        }

        /// <summary>
        /// Get the Element Id for this element
        /// </summary>
        public long Id
        {
            get
            {
                return InternalElementId.Value;
            }
        }

        /// <summary>
        /// Get the Element Unique Id for this element
        /// </summary>
        public string UniqueId
        {
            get
            {
                return InternalUniqueId;
            }
        }

        /// <summary>
        /// Get the Element Pinned status
        /// </summary>
        public bool IsPinned
        {
            get 
            {
                if (InternalElement == null)
                    return false;
                return this.InternalElement.Pinned; 
            }
        }

        /// <summary>
        /// Checks if two elements are joined
        /// </summary>
        /// <param name="otherElement">Second element to check</param>
        /// <returns>True if the two elements are joined, False otherwise</returns>
        public bool AreJoined(Element otherElement)
        {
            return JoinGeometryUtils.AreElementsJoined(
                Document,
                this.InternalElement,
                otherElement.InternalElement);
        }

        /// <summary>
        /// A reference to the element
        /// </summary>
        [SupressImportIntoVM]
        public abstract Autodesk.Revit.DB.Element InternalElement
        {
            get;
        }

        private ElementId internalId;

        /// <summary>
        /// Get Element Category
        /// </summary>
        public Category GetCategory
        {
            get { return new Category(this.InternalElement.Category); }
        }

        /// <summary>
        /// Returns the ElementType for this Element. Returns null if the Element cannot have an ElementType assigned.
        /// </summary>
        /// <returns name="ElementType">Element Type or Null.</returns>
        public ElementType ElementType
        {
            get
            {
                var typeId = this.InternalElement.GetTypeId();
                if (typeId == ElementId.InvalidElementId)
                    return null;
                
                var doc = DocumentManager.Instance.CurrentDBDocument;
                return doc.GetElement(typeId).ToDSType(true) as ElementType;
            }
        }

        /// <summary>
        /// The element id for this element
        /// </summary>
        protected ElementId InternalElementId
        {
            get
            {
                if (internalId == null)
                    return InternalElement != null ? InternalElement.Id : null;
                return internalId;
            }
            set
            {
                internalId = value;

                var elementManager = ElementIDLifecycleManager<long>.GetInstance();
                elementManager.RegisterAsssociation(Id, this);

            }
        }

        /// <summary>
        /// The unique id for this element
        /// </summary>
        protected string InternalUniqueId;

        /// <summary>
        /// Set the element's freeze state. If the node is set to freeze
        /// all the elements for that node will be set to freeze.
        /// </summary>
        public bool IsFrozen = false;

        /// <summary>
        /// Default implementation of dispose that removes the element from the
        /// document
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public virtual void Dispose()
        {
            // Do not cleanup Revit elements if we are shutting down Dynamo or
            // closing homeworkspace or the element itself is frozen.
            if (DisposeLogic.IsShuttingDown || DisposeLogic.IsClosingHomeworkspace || IsFrozen)
                return;
            
            bool didRevitDelete = ElementIDLifecycleManager<long>.GetInstance().IsRevitDeleted(Id);

            var elementManager = ElementIDLifecycleManager<long>.GetInstance();
            int remainingBindings = elementManager.UnRegisterAssociation(Id, this);

            // Do not delete Revit owned elements
            if (!IsRevitOwned && remainingBindings == 0 && !didRevitDelete)
            {
                if(this.InternalElement is View && InternalElement.IsValidObject)
                {
                    Autodesk.Revit.UI.UIDocument uIDocument = new Autodesk.Revit.UI.UIDocument(Document);
                    var openedViews = uIDocument.GetOpenUIViews().ToList();
                    var shouldClosedViews = openedViews.FindAll(x => InternalElement.Id == x.ViewId);
                    foreach (var v in shouldClosedViews)
                    {
                        if (uIDocument.GetOpenUIViews().ToList().Count() > 1)
                            v.Close();
                        else
                            throw new InvalidOperationException(string.Format(Properties.Resources.CantCloseLastOpenView, this.ToString()));
                    }
                }
                DocumentManager.Instance.DeleteElement(new ElementUUID(InternalUniqueId));
            }
            else
            {
                //This element has gone
                //but there was something else holding onto the Revit object so don't purge it

                internalId = null;
            }

        }

        /// <summary>
        /// A basic implementation of ToString for Elements
        /// </summary>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Implement Equals() method. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override bool Equals(object obj)
        {
            Element otherElement = obj as Element;
            if (otherElement == null)
            {
                return false;
            }

            return UniqueId.Equals(otherElement.UniqueId);
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            // As a default, return the standard string representation.
            // Override ToString with format information in children.
            return ToString();
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            // Do nothing. We implement this method only to prevent the GraphicDataProvider from
            // attempting to interrogate the public properties, some of which may require regeneration
            // or transactions and which must necessarily be threaded in a specific way.
        }

        /// <summary>
        /// Delete the element and any elements that are totally dependent upon the element. 
        /// </summary>
        /// <param name="element">The element to delete.</param>
        /// <returns>The list of element id's deleted, including any dependent elements.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if element is null.</exception>
        public static long[] Delete(Element element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // Collection of elements deleted
            long[] deletedElements = null;

            try
            {
                // Document to work with
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

                // Start the transaction
                TransactionManager.Instance.EnsureInTransaction(document);

                // Delete the element, collecting the id's deleted.
                deletedElements = document.Delete(element.InternalElementId)
                        .Select(x => x.Value).ToArray<long>();
            }
            finally
            {
                // handle transaction cleanup based on successful deletion or not. 
                if (deletedElements == null || deletedElements.Length == 0)
                    TransactionManager.Instance.ForceCloseTransaction();
                else
                    TransactionManager.Instance.TransactionTaskDone();
            }

            // return deleted elements
            return deletedElements; 
        }

        #region Get/Set Parameter
        /// <summary>
        /// Get a parameter by name of an element
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns></returns>
        private Autodesk.Revit.DB.Parameter GetParameterByName(string parameterName)
        {
            //
            // Parameter names are not unique on a given element. There are several valid cases where 
            // duplicated parameter names can be found in the Parameter Set.
            //
            // The most common ones are:
            // 
            // 1. Multiple built-in parameters with the same name
            // This is a common implementation pattern when a different parameter behavior is wanted
            // for different scopes. For example, lets say that you want a parameter to be writable
            // in the property palette but read-only in a schedule view. The easiest way to accomplish
            // this would be to add two parameters. One that is read-write and one that is read-only. 
            // These parameters will both have the same name and they will share the same getter. 
            //
            // 2. Built-in parameters and User parameters with the same name
            // This happens when a loadable family defines a user parameter with the same name
            // as a built-in parameter.
            //
            // Currently, we try to resolve this and get consistent results by
            // 1. Get all parameters for the given name
            // 2. Sort parameters by ElementId - This will give us built-in parameters first (ID's for built-ins are always < -1)
            // 3. If it exist: Use the first writable parameter
            // 4. Otherwise: Use the first read-only parameter
            //
            var allParams =
            InternalElement.Parameters.Cast<Autodesk.Revit.DB.Parameter>()
                .Where(x => string.CompareOrdinal(x.Definition.Name, parameterName) == 0)
                .OrderBy(x => x.Id.Value);

            var param = allParams.FirstOrDefault(x => x.IsReadOnly == false) ?? allParams.FirstOrDefault();

            return param;
        }

        /// <summary>
        /// Get the value of one of the element's parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter whose value you want to obtain.</param>
        /// <returns></returns>
        public object GetParameterValueByName(string parameterName)
        {
            var param = GetParameterByName(parameterName);
            
            if (param == null || !param.HasValue)
                return string.Empty;

            return Revit.Elements.InternalUtilities.ElementUtils.GetParameterValue(param);
        }

        /// <summary>
        /// Set one of the element's parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value.</param>
        public Element SetParameterByName(string parameterName, object value)
        {
            var param = GetParameterByName(parameterName);

            if (param == null)
                throw new Exception(Properties.Resources.ParameterNotFound);

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            var dynval = value as dynamic;
            Revit.Elements.InternalUtilities.ElementUtils.SetParameterValue(param, dynval);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }
        #endregion

        #region Overrides & Hidden In ActiveView
        /// <summary>
        /// Override the element's color in the active view.
        /// </summary>
        /// <param name="color">The color to apply to a solid fill on the element.</param>
        public Element OverrideColorInView(Color color)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            var view = DocumentManager.Instance.CurrentUIDocument.ActiveView;
            var ogs = new Autodesk.Revit.DB.OverrideGraphicSettings();

            var patternCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            patternCollector.OfClass(typeof(Autodesk.Revit.DB.FillPatternElement));
            Autodesk.Revit.DB.FillPatternElement solidFill = patternCollector.ToElements().Cast<Autodesk.Revit.DB.FillPatternElement>().First(x => x.GetFillPattern().IsSolidFill);

            var overrideColor = new Autodesk.Revit.DB.Color(color.Red, color.Green, color.Blue);
            ogs.SetSurfaceForegroundPatternColor(overrideColor);
            ogs.SetSurfaceForegroundPatternId(solidFill.Id);
            ogs.SetProjectionLineColor(overrideColor);
            view.SetElementOverrides(InternalElementId, ogs);
            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Override Elements Graphics Settings in Active View.
        /// </summary>
        /// <param name="overrides">Override Graphics Settings.</param>
        /// <param name="hide">If True given Element will be hidden.</param>
        /// <returns></returns>
        public Element OverrideInView(Revit.Filter.OverrideGraphicSettings overrides, bool hide = false)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            var view = DocumentManager.Instance.CurrentUIDocument.ActiveView;
            view.SetElementOverrides(InternalElementId, overrides.InternalOverrideGraphicSettings);
            if (hide) view.HideElements(new List<ElementId>() { InternalElementId });
            else view.UnhideElements(new List<ElementId>() { InternalElementId });
            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Gets graphic overrides for an element in the view. 
        /// </summary>
        public Revit.Filter.OverrideGraphicSettings OverridesInView
        {
            get
            {
                var view = DocumentManager.Instance.CurrentUIDocument.ActiveView;

                return new Filter.OverrideGraphicSettings(view.GetElementOverrides(InternalElementId));
            }
        }

        /// <summary>
        /// Identifies if the element has been permanently hidden in the view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool IsHiddeninView(Revit.Elements.Views.View view)
        {
            return InternalElement.IsHidden(view.InternalView);
        }

        #endregion

        /// <summary>
        /// Returns all geometry associated with an element. Ignores transforms when used with linked elements.
        /// </summary>
        /// <returns name='geometry[]'>List of geometry from the element</returns>
        public object[] Geometry()
        {
            var converted = new List<object>();

            foreach (var geometryObject in InternalGeometry())
            {
                var geoObj = geometryObject.Convert();
                if (geoObj != null)
                {
                    converted.Add(geoObj);
                }
                else
                {
                    var solid = geometryObject as Autodesk.Revit.DB.Solid;
                    if (solid != null)
                    {
                        var geomObjs = solid.ConvertToMany();

                        if (geomObjs == null) continue;

                        converted.AddRange(geomObjs.Where(x => { return x != null; }));
                    }
                }
            }

            return converted.ToArray();
        }

        /// <summary>
        /// Returns all geometry associated with an element. Ignores transforms when used with linked elements.
        /// <param name="detailLevel">Detail level</param>
        /// <returns>List of geometry from the element</returns>
        /// </summary>
        public object[] GetGeometry(string detailLevel = "Medium")
        {
            var converted = new List<object>();

            ViewDetailLevel dlevel;
            switch (detailLevel)
            {
                case "Coarse":
                    dlevel = ViewDetailLevel.Coarse;
                    break;
                case "Medium":
                    dlevel = ViewDetailLevel.Medium;
                    break;
                case "Fine":
                    dlevel = ViewDetailLevel.Fine;
                    break;
                default:
                    dlevel = ViewDetailLevel.Undefined;
                    break;
            }

            foreach (var geometryObject in InternalGeometry(false, dlevel))
            {
                var geoObj = geometryObject.Convert();
                if (geoObj != null)
                {
                    converted.Add(geoObj);
                }
                else
                {
                    var solid = geometryObject as Autodesk.Revit.DB.Solid;
                    if (solid != null)
                    {
                        var geomObjs = solid.ConvertToMany();

                        if (geomObjs == null) continue;

                        converted.AddRange(geomObjs.Where(x => { return x != null; }));
                    }
                }
            }

            return converted.ToArray();
        }

        /// <summary>
        /// Gets all elements hosted by the supplied element
        /// </summary>
        /// <param name="includeOpenings">Include rectangular openings in output</param>
        /// <param name="includeHostedElementsOfJoinedHosts">Include the hosted elements from the multiple joined hosts in output</param>
        /// <param name="includeEmbeddedWalls">Include embedded walls in output</param>
        /// <param name="includeSharedEmbeddedInserts">Include shared embedded elements in output</param>
        /// <returns>Hosted Elements</returns>
        public IEnumerable<Element> GetHostedElements(
            bool includeOpenings = false,
            bool includeHostedElementsOfJoinedHosts = false,
            bool includeEmbeddedWalls = false,
            bool includeSharedEmbeddedInserts = false)
        {

            var hostObject = this.InternalElement as HostObject;
            if (hostObject == null)
                throw new NullReferenceException(Properties.Resources.NotHostElement);

            IList<ElementId> inserts = hostObject
                .FindInserts(includeOpenings,
                             includeHostedElementsOfJoinedHosts,
                             includeEmbeddedWalls,
                             includeSharedEmbeddedInserts);

            // Get all hosted elements from their Id's 
            // and convert them to DS type
            List<Element> hostedElements = new List<Element>();
            for (int i = 0; i < inserts.Count; i++)
            {
                Element elem = Document.GetElement(inserts[i]).ToDSType(true);
                hostedElements.Add(elem);
            }
            return hostedElements;
        }

        /// <summary>
        /// Sets an existing element's pinned status
        /// </summary>
        /// <param name="pinned">Value for pin status true/false</param>
        public Element SetPinnedStatus(bool pinned)
        {
            if (this.InternalElement.Pinned == pinned) return this;

            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalElement.Pinned = pinned;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #region Geometry extraction

        /// <summary>
        /// Extract the Revit GeometryObject's from a Revit Element
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Autodesk.Revit.DB.GeometryObject> InternalGeometry(bool useSymbolGeometry = false,
            ViewDetailLevel detailLevel = ViewDetailLevel.Medium)
        {
            DocumentManager.Regenerate();

            var thisElement = InternalElement;

            var goptions0 = new Options { ComputeReferences = true, DetailLevel = detailLevel };

            var geomElement = thisElement.get_Geometry(goptions0);

            // GenericForm is a special case
            if ((thisElement is GenericForm) && (!geomElement.Any()))
            {
                var gF = (GenericForm)thisElement;
                if (!gF.Combinations.IsEmpty)
                {
                    var goptions1 = new Options
                    {
                        IncludeNonVisibleObjects = true,
                        ComputeReferences = true
                    };
                    geomElement = thisElement.get_Geometry(goptions1);
                }
            }

            // FamilySymbol is a special case: the symbol must be activated before geometry is available, unless an instance is already present in the model
            else if ((thisElement is FamilySymbol) && (geomElement == null || !geomElement.Any()))
            {
                var fs = (FamilySymbol)thisElement;
                if (!fs.IsActive)
                {
                    fs.Activate();
                    DocumentManager.Regenerate();
                    geomElement = fs.get_Geometry(goptions0);
                }

            }

            return CollectConcreteGeometry(geomElement, useSymbolGeometry);
        }

        /// <summary>
        /// Collects the concrete GeometryObject's in a GeometryElement, which is a recursive collection of GeometryObject's.
        /// </summary>
        /// <param name="geometryElement">The Geometry collection</param>
        /// <param name="useSymbolGeometry">When encountering a GeometryInstance, use GetSymbolGeometry() which obtains usable Reference objects</param>
        /// <returns></returns>
        private static IEnumerable<GeometryObject> CollectConcreteGeometry(GeometryElement geometryElement, bool useSymbolGeometry = false)
        {
            var instanceGeometryObjects = new List<GeometryObject>();

            if (geometryElement == null) return instanceGeometryObjects;

            foreach (GeometryObject geob in geometryElement)
            {
                var geomInstance = geob as GeometryInstance;
                var geomElement = geob as GeometryElement;

                if (geomInstance != null)
                {
                    var instanceGeom = useSymbolGeometry ? geomInstance.GetSymbolGeometry() : geomInstance.GetInstanceGeometry();
                    instanceGeometryObjects.AddRange(CollectConcreteGeometry(instanceGeom));
                }
                else if (geomElement != null)
                {
                    instanceGeometryObjects.AddRange(CollectConcreteGeometry(geometryElement));
                }
                else
                {
                    instanceGeometryObjects.Add(geob);
                }
            }

            // Certain kinds of Elements will return Solids with zero faces - make sure to filter them out
            return
                instanceGeometryObjects.Where(
                    x =>
                        !(x is Autodesk.Revit.DB.Solid) || (x as Autodesk.Revit.DB.Solid).Faces.Size > 0);
        }

        /// <summary>
        /// A generic method extract all GeometryObject's of the supplied type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<T> InternalGeometry<T>(bool useSymbolGeometry = false) where T : GeometryObject
        {
            return this.InternalGeometry(useSymbolGeometry).OfType<T>();
        }

        /// <summary>
        /// The Solids in this Element
        /// </summary>
        public Autodesk.DesignScript.Geometry.Solid[] Solids
        {
            get
            {
                return
                    this.InternalGeometry<Autodesk.Revit.DB.Solid>()
                        .Select(x => x.ToProtoType())
                        .ToArray();
            }
        }

        /// <summary>
        /// The Curves in this Element
        /// </summary>
        public Curve[] Curves
        {
            get
            {
                // This is the correctly translated geometry, obtained from GetInstanceGeometry
                var geoms = this.InternalGeometry<Autodesk.Revit.DB.Curve>();

                // The is the geometry with the correctly computed References, from GetSymbolGeometry
                var refs = InternalGeometry<Autodesk.Revit.DB.Curve>(true).Select(x => x.Reference);

                return geoms.Zip(refs, (geom, reference) => geom.ToProtoType(true, reference))
                    .ToArray();
            }
        }

        /// <summary>
        /// The Faces in this Element
        /// </summary>
        public Surface[] Faces
        {
            get
            {
                // This is the correctly translated geometry, obtained from GetInstanceGeometry
                var geoms = InternalGeometry<Autodesk.Revit.DB.Solid>()
                    .SelectMany(x => x.Faces.OfType<Autodesk.Revit.DB.Face>());

                // The is the geometry with the correctly computed References, from GetSymbolGeometry
                var refs = InternalGeometry<Autodesk.Revit.DB.Solid>(true)
                    .SelectMany(x => x.Faces.OfType<Autodesk.Revit.DB.Face>())
                    .Select(x => x.Reference);

                return
                    geoms.Zip(refs, (geom, reference) => geom.ToProtoType(true, reference))
                        .SelectMany(x => x).ToArray();
            }
        }

        /// <summary>
        /// The ElementCurveReference's in this Element.  Useful for downstream
        /// Element creation.
        /// </summary>
        public ElementCurveReference[] ElementCurveReferences
        {
            get
            {
                return
                    this.InternalGeometry<Autodesk.Revit.DB.Curve>(true)
                        .Select(ElementCurveReference.FromExisting)
                        .ToArray();
            }
        }

        /// <summary>
        /// The ElementFaceReference's in this Element.  Useful for downstream
        /// Element creation.
        /// </summary>
        public ElementFaceReference[] ElementFaceReferences
        {
            get
            {
                return
                    this.InternalGeometry<Autodesk.Revit.DB.Solid>(true)
                        .SelectMany(x => x.Faces.OfType<Autodesk.Revit.DB.Face>())
                        .Select(ElementFaceReference.FromExisting)
                        .ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Is this element still alive in Revit, and good to be drawn, queried etc.
        /// </summary>
        protected bool IsAlive
        {
            get
            {
                if (InternalElementId == null)
                {
                    return false;
                }

                //Ensure that the object is still alive

                //Check whether the internal element Id is null
                if (null == InternalElementId)
                    return false;

                return !ElementIDLifecycleManager<long>.GetInstance().IsRevitDeleted(InternalElementId.Value);
            }
        }

        #region Get Child Elements
        /// <summary>
        /// Gets the child Elements of the current Element.
        /// </summary>
        /// <returns>Child Elements.</returns>
        public IEnumerable<Element> GetChildElements()
        {
            return GetElementChildren(this.InternalElement);
            
        }

        private IEnumerable<Element> GetElementChildren(Autodesk.Revit.DB.Element element)
        {
            List<Element> components = new List<Element>();
            BuiltInCategory builtInCategory = element.Category.BuiltInCategory;
            if (builtInCategory == BuiltInCategory.INVALID)
                throw new InvalidOperationException(Properties.Resources.NotBuiltInCategory);

            // By default we use the GetSubComponentIds() on the elements FamilyInstance, 
            // for now the node also handles special cases including Stairs, StructuralFramingSystems and Railings 
            switch (builtInCategory)
            {
                case BuiltInCategory.OST_Stairs:
                    List<Element> stairComponentElements = GetChildElementsFromStairs(element);
                    components.AddRange(stairComponentElements);
                    break;

                case BuiltInCategory.OST_StructuralFramingSystem:
                    List<Element> beamSystemComponentElements = GetChildElementsFromStructuralFramingSystem(element);
                    components.AddRange(beamSystemComponentElements);
                    break;

                case BuiltInCategory.OST_StairsRailing:
                    List<Element> railingComponentElements = GetChildElementsFromRailings(element);
                    components.AddRange(railingComponentElements);
                    break;

                default:
                    List<Element> componentElements = GetChildElementsFromFamilyInstance(element);
                    components.AddRange(componentElements);
                    break;
            }
            return components;
        }

        private static List<Element> GetChildElementsFromFamilyInstance(Autodesk.Revit.DB.Element element)
        {
            var familyInstance = element as Autodesk.Revit.DB.FamilyInstance;
            if (familyInstance == null)
                throw new NullReferenceException(Properties.Resources.ChildElementsNotSupported);

            List<ElementId> componentIds = familyInstance.GetSubComponentIds().ToList();
            if (componentIds.Count == 0 || componentIds == null)
                throw new NullReferenceException(Properties.Resources.NoChildElements);

            List<Element> componentElements = componentIds.Select(id => Document.GetElement(id).ToDSType(true))
                                                           .ToList();
            return componentElements;
        }

        private static List<Element> GetChildElementsFromRailings(Autodesk.Revit.DB.Element element)
        {
            var railingElement = element as Autodesk.Revit.DB.Architecture.Railing;
            List<ElementId> railingComponentIds = new List<ElementId>();
            // For Railings we get the HandRails and the TopRail as child elements 
            railingComponentIds.AddRange(railingElement.GetHandRails().ToList());
            railingComponentIds.Add(railingElement.TopRail);

            if (railingComponentIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoChildElements);

            List<Element> railingComponentElements = railingComponentIds.Select(id => Document.GetElement(id).ToDSType(true))
                                                                         .ToList();
            return railingComponentElements;
        }

        private static List<Element> GetChildElementsFromStructuralFramingSystem(Autodesk.Revit.DB.Element element)
        {
            var beamSystemElement = element as Autodesk.Revit.DB.BeamSystem;
            // For Beam systems we get each indivdual Beam as the childs
            List<ElementId> beamSystemComponentIds = beamSystemElement.GetBeamIds().ToList();

            if (beamSystemComponentIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoChildElements);

            List<Element> beamSystemComponentElements = beamSystemComponentIds
                .Select(id => Document.GetElement(id).ToDSType(true))
                .ToList();
            return beamSystemComponentElements;
        }

        private static List<Element> GetChildElementsFromStairs(Autodesk.Revit.DB.Element element)
        {
            var stairElement = element as Autodesk.Revit.DB.Architecture.Stairs;
            List<ElementId> stairComponentIds = new List<ElementId>();

            // For stairs we get Landings, Runs and Supports as the child Elements,
            // using the following methods.
            stairComponentIds.AddRange(stairElement.GetStairsLandings().ToList());
            stairComponentIds.AddRange(stairElement.GetStairsRuns().ToList());
            stairComponentIds.AddRange(stairElement.GetStairsSupports().ToList());
            if (stairComponentIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoChildElements);

            List<Element> stairComponentElements = stairComponentIds.Select(id => Document.GetElement(id).ToDSType(true))
                                                                     .ToList();
            return stairComponentElements;
        }
        #endregion

        #region Get Parent Element
        /// <summary>
        /// Gets the parent element of the Element.
        /// </summary>
        /// <returns>Parent Element</returns>
        public Element GetParentElement()
        {
            return GetParentElementFromElement(this.InternalElement);
        }

        private Element GetParentElementFromElement(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            BuiltInCategory builtInCategory = element.Category.BuiltInCategory;
            if (builtInCategory == BuiltInCategory.INVALID)
                throw new InvalidOperationException(Properties.Resources.NotBuiltInCategory);

            // By default we use the SuperComponent on the elements FamilyInstance to get the Parent Element, 
            // for now the node also handles special cases including Stairs, StructuralFramingSystems and Railings 
            switch (builtInCategory)
            {
                case BuiltInCategory.OST_StairsLandings:
                    parent = GetParentComponentFromStairElements(element);
                    break;

                case BuiltInCategory.OST_StairsRuns:
                    parent = GetParentComponentFromStairElements(element);
                    break;

                case BuiltInCategory.OST_StructuralFraming:
                    parent = GetParentElementFromStructuralFraming(element);
                    break;

                case BuiltInCategory.OST_RailingHandRail:
                    parent = GetParentElementFromRailingElements(element);
                    break;

                case BuiltInCategory.OST_RailingTopRail:
                    parent = GetParentElementFromRailingElements(element);
                    break;

                default:
                    parent = GetParentElementFromFamilyInstance(element);
                    break;
            }
            if (parent == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);
            return parent.ToDSType(true);
        }

        private static Autodesk.Revit.DB.Element GetParentElementFromFamilyInstance(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var familyInstance = element as Autodesk.Revit.DB.FamilyInstance;

            if (familyInstance == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);

            parent = familyInstance.SuperComponent;
            return parent;
        }

        private static Autodesk.Revit.DB.Element GetParentElementFromRailingElements(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var railingElement = element as Autodesk.Revit.DB.Architecture.ContinuousRail;
            ElementId hostId = railingElement.HostRailingId;

            if (hostId == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);

            // For Railings we return the Host as the Parent element
            parent = Document.GetElement(hostId);
            return parent;
        }

        private static Autodesk.Revit.DB.Element GetParentElementFromStructuralFraming(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var beam = element as Autodesk.Revit.DB.FamilyInstance;

            if (beam == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);

            // For Beam systems we get each indivdual Beam as the childs
            parent = BeamSystem.BeamBelongsTo(beam);
            return parent;
        }

        private static Autodesk.Revit.DB.Element GetParentComponentFromStairElements(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;

            if(element is Autodesk.Revit.DB.Architecture.StairsLanding)
            {
                var stairElement = element as Autodesk.Revit.DB.Architecture.StairsLanding;
                parent = stairElement.GetStairs();
            }
            else if(element is Autodesk.Revit.DB.Architecture.StairsRun)
            {
                var stairElement = element as Autodesk.Revit.DB.Architecture.StairsRun;
                parent = stairElement.GetStairs();
            }
            else
                throw new InvalidOperationException(Properties.Resources.NoParentElement);

            return parent;
        }
        #endregion

        #region Geometry Join
        /// <summary>
        /// Finds the elements whose geometry is joined with the given element.
        /// </summary>
        /// <returns>All elements whose geometry is joined to the given element.</returns>
        public IEnumerable<Element> GetJoinedElements()
        {
            return JoinGeometryUtils.GetJoinedElements(Document, this.InternalElement)
                .Select(id => Document.GetElement(id).ToDSType(true))
                .ToList();
        }

        /// <summary>
        /// Gets all Elements intersecting the input element, that are of a specific category.
        /// </summary>
        /// <param name="category">Category of Elements to check intersection against</param>
        /// <returns>List of intersection elements of the specified category</returns>
        public IEnumerable<Element> GetIntersectingElementsOfCategory(Category category)
        {
            BuiltInCategory builtInCategory = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                 category.InternalCategory.Id.ToString());

            ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(this.InternalElement);
            FilteredElementCollector intersecting = new FilteredElementCollector(Document).WherePasses(filter)
                                                                                          .OfCategory(builtInCategory);
            if (!intersecting.Any())
                return new List<Element>();

            return intersecting.Select(x => x.ToDSType(true)).ToList();
        }

        /// <summary>
        /// Unjoin the geometry of two Elements.
        /// </summary>
        /// <param name="otherElement">Other element to unjoin from the element.</param>
        /// <returns>The input elements with their geometry unjoined.</returns>
        public IEnumerable<Element> UnjoinGeometry(Element otherElement)
        {
            if (!JoinGeometryUtils.AreElementsJoined(Document, this.InternalElement, otherElement.InternalElement))
                throw new InvalidOperationException(Properties.Resources.NotJoinedElements);

            TransactionManager.Instance.EnsureInTransaction(Document);
            JoinGeometryUtils.UnjoinGeometry(
                        Document,
                        this.InternalElement,
                        otherElement.InternalElement);
            TransactionManager.Instance.TransactionTaskDone();
            return new List<Element>() { this, otherElement };
        }

        /// <summary>
        /// Unjoins the geometry of all elements from each other if they are joined.
        /// This performs only one transaction in Revit.
        /// </summary>
        /// <param name="elements">List of elements to unjoin from each other</param>
        /// <returns>All input Elements, with their geometry now unjoined from each other.</returns>
        public static IEnumerable<Element> UnjoinAllGeometry(List<Element> elements)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            for (int i = 0; i < elements.Count; i++)
            {
                List<Element> joinedElements = JoinGeometryUtils.GetJoinedElements(Document, elements[i].InternalElement)
                                                                .Select(id => Document.GetElement(id).ToDSType(true))
                                                                .ToList();
                if (joinedElements.Count <= 0)
                    continue;

                for (int j = 0; j < joinedElements.Count; j++)
                {
                    JoinGeometryUtils.UnjoinGeometry(
                        Document,
                        elements[i].InternalElement,
                        joinedElements[j].InternalElement);
                }
            }
            TransactionManager.Instance.TransactionTaskDone();
            return elements;
        }
        
        /// <summary>
        /// Sets the order in which the geometry of two elements is joined.
        /// </summary>
        /// <param name="cuttingElement">The element that should be cutting the other element</param>
        /// <param name="otherElement">The other element that is being cut by the cuttingElement</param>
        /// <returns>Input elements with the geometry join order updated.</returns>
        public static IEnumerable<Element> SetGeometryJoinOrder(Element cuttingElement, Element otherElement)
        {
            if (!JoinGeometryUtils.AreElementsJoined(Document, cuttingElement.InternalElement, otherElement.InternalElement))
            {
                throw new InvalidOperationException(Properties.Resources.InvalidSwitchJoinOrder);
            }
            if (JoinGeometryUtils.IsCuttingElementInJoin(Document, cuttingElement.InternalElement, otherElement.InternalElement))
            {
                return new List<Element>() { cuttingElement, otherElement };
            }         
            TransactionManager.Instance.EnsureInTransaction(Document);
            JoinGeometryUtils.SwitchJoinOrder(Document, cuttingElement.InternalElement, otherElement.InternalElement);
            TransactionManager.Instance.TransactionTaskDone();
            return new List<Element>() { cuttingElement, otherElement };
        }
        
        /// <summary>
        /// Joins the geometry of two elements, if they are intersecting.
        /// </summary>
        /// <param name="otherElement">Other element to join with</param>
        /// <returns>The two joined elements</returns>
        public IEnumerable<Element> JoinGeometry(Element otherElement)
        {
            var joinedElements = new List<Element>() { this, otherElement };
            if (JoinGeometryUtils.AreElementsJoined(Document, this.InternalElement, otherElement.InternalElement))
                return joinedElements;

            ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(otherElement.InternalElement);
            ICollection<Autodesk.Revit.DB.Element> collector = new FilteredElementCollector(Document).WherePasses(filter).ToElements();

            if (collector.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NonIntersectingElements);

            TransactionManager.Instance.EnsureInTransaction(Document);
            JoinGeometryUtils.JoinGeometry(Document, this.InternalElement, otherElement.InternalElement);
            TransactionManager.Instance.TransactionTaskDone();

            return joinedElements;
        }
        #endregion

        #region Location extraction & manipulation
        /// <summary>
        /// Update an existing element's location
        /// </summary>
        /// <param name="geometry">New Location Point or Curve</param>
        public void SetLocation(Geometry geometry)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);

            if (this.InternalElement.Location is Autodesk.Revit.DB.LocationPoint)
            {
                if (geometry is Autodesk.DesignScript.Geometry.Point)
                {
                    Autodesk.DesignScript.Geometry.Point point = geometry as Autodesk.DesignScript.Geometry.Point;
                    Autodesk.Revit.DB.LocationPoint pt = this.InternalElement.Location as Autodesk.Revit.DB.LocationPoint;
                    pt.Point = point.ToRevitType(true);
                }
                else
                    throw new Exception(Properties.Resources.PointRequired);
            }
            else if (this.InternalElement.Location is Autodesk.Revit.DB.LocationCurve && geometry is Curve)
            {
                if (geometry is Curve)
                {
                    Curve dynamoCurve = geometry as Curve;
                    Autodesk.Revit.DB.LocationCurve curve = this.InternalElement.Location as Autodesk.Revit.DB.LocationCurve;
                    curve.Curve = dynamoCurve.ToRevitType(true);
                }
                else
                    throw new Exception(Properties.Resources.CurveRequired);
            }
            else
            {
                throw new Exception(Properties.Resources.InvalidElementLocation);
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Returns an element's location
        /// </summary>
        /// <returns name="geometry[]">The element’s location</returns>
        public Geometry GetLocation()
        {
            if (this.InternalElement.Location is Autodesk.Revit.DB.LocationPoint)
            {
                Autodesk.Revit.DB.LocationPoint pt = this.InternalElement.Location as Autodesk.Revit.DB.LocationPoint;
                return pt.Point.ToPoint(true);
            }
            else if (this.InternalElement.Location is Autodesk.Revit.DB.LocationCurve)
            {
                Autodesk.Revit.DB.LocationCurve curve = this.InternalElement.Location as Autodesk.Revit.DB.LocationCurve;
                return curve.Curve.ToProtoType(true);
            }
            else
            {
                throw new Exception(Properties.Resources.InvalidElementLocation);
            }
        }

        /// <summary>
        /// Move Revit Element by Vector
        /// </summary>
        /// <param name="vector">Translation Vector</param>
        public void MoveByVector(Vector vector)
        {
            if (!this.InternalElement.Location.Move(vector.ToXyz(true)))
            {
                throw new Exception(Properties.Resources.InvalidElementLocation);
            }
        }

        #endregion

        #region Material

        /// <summary>
        /// Get Material Names from a Revit Element
        /// </summary>
        /// <param name="paintMaterials">Paint Materials</param>
        /// <returns>List of Names</returns>
        public IEnumerable<Material> GetMaterials(bool paintMaterials = false)
        {
            // Get the active Document
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

            List<Material> materialnames = new List<Material>();

            foreach (Autodesk.Revit.DB.ElementId id in this.InternalElement.GetMaterialIds(paintMaterials))
            {
                Autodesk.Revit.DB.Material material = (Autodesk.Revit.DB.Material)document.GetElement(id);
                Material mat = Material.FromExisting(material, true);

                if (!materialnames.Contains(mat)) materialnames.Add(mat);
            }

            return materialnames;
        }

        #endregion

        #region Subelement
        /// <summary>
        /// Obtain all of the Subelements from an Element.
        /// </summary>
        public Subelement[] Subelements
        {
            get
            {
                return
                    InternalElement.GetSubelements().Cast<Autodesk.Revit.DB.Subelement>()
                        .Select(x => new Subelement(x))
                        .ToArray();
            }
        }
        #endregion
    }
}

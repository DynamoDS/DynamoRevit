using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using DynamoUnits;
using Revit.Elements.InternalUtilities;
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

            var elementManager = ElementIDLifecycleManager<int>.GetInstance();
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Element>(Document);
            int count = 0;
            int id = 0;
            if (null != element)
            {
                id = element.Id.IntegerValue;
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
                TransactionManager.Instance.TransactionTaskDone();
                return bb.ToProtoType();
            }
        }

        /// <summary>
        /// Get the Element Id for this element
        /// </summary>
        public int Id
        {
            get
            {
                return InternalElementId.IntegerValue;
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
        /// Returns the FamilyType for this Element. Returns null if the Element cannot have a FamilyType assigned.
        /// </summary>
        /// <returns name="ElementType">Element Type or Null.</returns>
        public Element ElementType
        {
            get
            {
                var typeId = this.InternalElement.GetTypeId();
                if (typeId == ElementId.InvalidElementId)
                {
                    return null;
                }
                else
                {
                    var doc = DocumentManager.Instance.CurrentDBDocument;
                    return doc.GetElement(typeId).ToDSType(true);
                }
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

                var elementManager = ElementIDLifecycleManager<int>.GetInstance();
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
            
            bool didRevitDelete = ElementIDLifecycleManager<int>.GetInstance().IsRevitDeleted(Id);

            var elementManager = ElementIDLifecycleManager<int>.GetInstance();
            int remainingBindings = elementManager.UnRegisterAssociation(Id, this);

            // Do not delete Revit owned elements
            if (!IsRevitOwned && remainingBindings == 0 && !didRevitDelete)
            {
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
        /// <param name="obj"></param>
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
        public static int[] Delete(Element element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // Collection of elements deleted
            int[] deletedElements = null;

            try
            {
                // Document to work with
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

                // Start the transaction
                TransactionManager.Instance.EnsureInTransaction(document);

                // Delete the element, collecting the id's deleted.
                deletedElements = document.Delete(element.InternalElementId)
                        .Select(x => x.IntegerValue).ToArray<int>();
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
                .OrderBy(x => x.Id.IntegerValue);

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



        /// <summary>
        /// Get all of the Geometry associated with this object
        /// </summary>
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

        #region Geometry extraction

        /// <summary>
        /// Extract the Revit GeometryObject's from a Revit Element
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Autodesk.Revit.DB.GeometryObject> InternalGeometry(bool useSymbolGeometry = false)
        {
            DocumentManager.Regenerate();

            var thisElement = InternalElement;

            var goptions0 = new Options { ComputeReferences = true };

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

                return !ElementIDLifecycleManager<int>.GetInstance().IsRevitDeleted(InternalElementId.IntegerValue);
            }
        }

        /// <summary>
        /// Gets the child Elements of the current Element.
        /// </summary>
        /// <returns>Child Elements.</returns>
        public IEnumerable<Element> GetChildElements()
        {
            return GetElementChildElements(this.InternalElement);
            
        }

        private IEnumerable<Element> GetElementChildElements(Autodesk.Revit.DB.Element element)
        {
            List<Element> components = new List<Element>();
            BuiltInCategory builtInCategory = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                 element.Category.Id.ToString());

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
                throw new NullReferenceException(Properties.Resources.NoChildElements);
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
            List<ElementId> beamSystemComponentIds = beamSystemElement.GetBeamIds().ToList();
            if (beamSystemComponentIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoChildElements);
            List<Element> beamSystemComponentElements = beamSystemComponentIds.Select(id => Document.GetElement(id).ToDSType(true))
                                                                     .ToList();
            return beamSystemComponentElements;
        }

        private static List<Element> GetChildElementsFromStairs(Autodesk.Revit.DB.Element element)
        {
            var stairElement = element as Autodesk.Revit.DB.Architecture.Stairs;
            List<ElementId> stairComponentIds = new List<ElementId>();
            stairComponentIds.AddRange(stairElement.GetStairsLandings().ToList());
            stairComponentIds.AddRange(stairElement.GetStairsRuns().ToList());
            stairComponentIds.AddRange(stairElement.GetStairsSupports().ToList());
            if (stairComponentIds.Count == 0)
                throw new InvalidOperationException(Properties.Resources.NoChildElements);
            List<Element> stairComponentElements = stairComponentIds.Select(id => Document.GetElement(id).ToDSType(true))
                                                                     .ToList();
            return stairComponentElements;
        }

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
            BuiltInCategory builtInCategory = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                 element.Category.Id.ToString());
            switch (builtInCategory)
            {
                case BuiltInCategory.OST_StairsLandings:
                    parent = GetParentComponentFromStairsLandings(element);
                    break;

                case BuiltInCategory.OST_StructuralFraming:
                    parent = GetParentElementFromStructuralFraming(element);
                    break;

                case BuiltInCategory.OST_Railings:
                    parent = GetParentElementFromRailings(element);
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

        private static Autodesk.Revit.DB.Element GetParentElementFromRailings(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var railingElement = element as Autodesk.Revit.DB.Architecture.ContinuousRail;
            ElementId hostId = railingElement.HostRailingId;
            if (hostId == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);
            parent = Document.GetElement(hostId);
            return parent;
        }

        private static Autodesk.Revit.DB.Element GetParentElementFromStructuralFraming(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var beam = element as Autodesk.Revit.DB.FamilyInstance;
            if (beam == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);
            parent = BeamSystem.BeamBelongsTo(beam);
            return parent;
        }

        private static Autodesk.Revit.DB.Element GetParentComponentFromStairsLandings(Autodesk.Revit.DB.Element element)
        {
            Autodesk.Revit.DB.Element parent;
            var stairElement = element as Autodesk.Revit.DB.Architecture.StairsLanding;
            if (stairElement == null)
                throw new InvalidOperationException(Properties.Resources.NoParentElement);
            parent = stairElement.GetStairs();
            return parent;
        }

        /// <summary>
        /// Transforms this Element from source CoordinateSystem to new context CoordinateSystem.
        /// </summary>
        /// <param name="fromCoordinateSystem">Source CoordinatSystem</param>
        /// <param name="contextCoordinateSystem">Context CordinateSystem</param>
        /// <returns>Transformed Element</returns>
        public Element Transform(CoordinateSystem fromCoordinateSystem, CoordinateSystem contextCoordinateSystem)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            var transManager = TransactionManager.Instance.TransactionWrapper;
            var t = transManager.StartTransaction(Document);
            try
            {
                SetLocationFromCS(this, fromCoordinateSystem, contextCoordinateSystem);
                RotateElementFromCS(this, fromCoordinateSystem, contextCoordinateSystem);
                t.CommitTransaction();
            }
            catch (Exception ex)
            {
                t.CancelTransaction();
                throw new Exception(ex.Message);
            }         
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        private void RotateElementFromCS(Element element, CoordinateSystem fromCS, CoordinateSystem contextCS)
        {
            var familyInstance = element as FamilyInstance;
            if (familyInstance == null)
                throw new NullReferenceException(nameof(element));
            var newTransform = contextCS.ToTransform() as Autodesk.Revit.DB.Transform;

            var oldTransform = fromCS.ToTransform() as Autodesk.Revit.DB.Transform;
            double[] oldRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(oldTransform, out oldRotationAngles);
            double[] newRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(newTransform, out newRotationAngles);
            // Convert Eular angle to degrees
            var rotationDegrees = (newRotationAngles.FirstOrDefault() / (2 * Math.PI)) * 360;
            var newRotationAngle = rotationDegrees * Math.PI / 180;

            if (!oldRotationAngles[0].AlmostEquals(newRotationAngle, 1.0e-6))
            {
                var rotateAngle = newRotationAngle - oldRotationAngles[0];
                var axis = Autodesk.Revit.DB.Line.CreateUnbound(oldTransform.Origin, oldTransform.BasisZ);
                Autodesk.Revit.DB.ElementTransformUtils.RotateElement(Document, new Autodesk.Revit.DB.ElementId(Id), axis, -rotateAngle);
            }
        }

        private void SetLocationFromCS(Element element, CoordinateSystem fromCS, CoordinateSystem contextCS)
        {
            var locationGeometry = element.InternalElement.Location;
            if (locationGeometry is Autodesk.Revit.DB.LocationCurve)
            {
                var locationCurve = locationGeometry as Autodesk.Revit.DB.LocationCurve;
                var dynamoCurve = locationCurve.Curve.ToProtoType();
                var newLocation = dynamoCurve.Transform(fromCS,contextCS) as Curve;
                locationCurve.Curve = newLocation.ToRevitType(true);
                dynamoCurve.Dispose();
                newLocation.Dispose();
                return;
            }
            else if (locationGeometry is Autodesk.Revit.DB.LocationPoint)
            {
                var location = element.InternalElement.Location as Autodesk.Revit.DB.LocationPoint;
                var dynamoPoint = location.Point.ToPoint();
                var newLocation = dynamoPoint.Transform(fromCS, contextCS) as Autodesk.DesignScript.Geometry.Point;
                location.Point = newLocation.ToRevitType(true);
                dynamoPoint.Dispose();
                newLocation.Dispose();
                return;
            }
            throw new Exception(Properties.Resources.InvalidElementLocation);
        }

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
        /// Get an existing element's location
        /// </summary>
        /// <returns>Location Geometry</returns>
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
    }
}

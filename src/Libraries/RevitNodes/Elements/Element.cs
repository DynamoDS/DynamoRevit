﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using DynamoServices;

using DynamoUnits;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Color = DSCore.Color;
using Area = DynamoUnits.Area;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Surface = Autodesk.DesignScript.Geometry.Surface;
using RevitServices.Elements;

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
        /// Obtain all of the Parameters from an Element
        /// </summary>
        public Parameter[] Parameters
        {
            get
            {
                var parms = InternalElement.Parameters;
                return parms.Cast<Autodesk.Revit.DB.Parameter>().Select(x => new Parameter(x)).ToArray();
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

        internal static bool IsConvertableParameterType(ParameterType paramType)
        {
            return paramType == ParameterType.Length || paramType == ParameterType.Area ||
                paramType == ParameterType.Volume || paramType == ParameterType.Angle ||
                paramType == ParameterType.Slope || paramType == ParameterType.Currency ||
                paramType == ParameterType.MassDensity;
        }

        internal static UnitType ParameterTypeToUnitType(ParameterType parameterType)
        {
            switch (parameterType)
            {
                case ParameterType.Length:
                    return UnitType.UT_Length;
                case ParameterType.Area:
                    return UnitType.UT_Area;
                case ParameterType.Volume:
                    return UnitType.UT_Volume;
                case ParameterType.Angle:
                    return UnitType.UT_Angle;
                case ParameterType.Slope:
                    return UnitType.UT_Slope;
                case ParameterType.Currency:
                    return UnitType.UT_Currency;
                case ParameterType.MassDensity:
                    return UnitType.UT_MassDensity;
                default:
                    throw new Exception(Properties.Resources.UnitTypeConversionError);
            }
        }

        /// <summary>
        /// Get the value of one of the element's parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter whose value you want to obtain.</param>
        /// <returns></returns>
        public object GetParameterValueByName(string parameterName)
        {
            object result;

            var param =
                // We don't use Element.GetOrderedParameters(), it only returns ordered parameters
                // as show in the UI
                InternalElement.Parameters.Cast<Autodesk.Revit.DB.Parameter>()
                    // Element.Parameters returns a differently ordered list on every invocation.
                    // We must sort it to get sensible results.
                    .OrderBy(x => x.Id.IntegerValue) 
                    .FirstOrDefault(x => x.Definition.Name == parameterName);         
            
            if (param == null || !param.HasValue)
                return string.Empty;

            switch (param.StorageType)
            {
                case StorageType.ElementId:
                    int valueId = param.AsElementId().IntegerValue;
                    if (valueId > 0)
                    {
                        // When the element is obtained here, to convert it to our element wrapper, it
                        // need to be figured out whether this element is created by us. Here the existing
                        // element wrappers will be checked. If there is one, its property to specify
                        // whether it is created by us will be followed. If there is none, it means the
                        // element is not created by us.
                        var elem = ElementIDLifecycleManager<int>.GetInstance().GetFirstWrapper(valueId) as Element;
                        result = ElementSelector.ByElementId(valueId, elem == null ? true : elem.IsRevitOwned);
                    }
                    else
                    {
                        int paramId = param.Id.IntegerValue;
                        if (paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM || paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
                        {
                            var categories = DocumentManager.Instance.CurrentDBDocument.Settings.Categories;
                            result = new Category(categories.get_Item((BuiltInCategory)valueId));
                        }
                        else
                        {
                            // For other cases, return a localized string
                            result = param.AsValueString();
                        }
                    }
                    break;
                case StorageType.String:
                    result = param.AsString();
                    break;
                case StorageType.Integer:
                    result = param.AsInteger();
                    break;
                case StorageType.Double:
                    var paramType = param.Definition.ParameterType;
                    if (IsConvertableParameterType(paramType))
                        result = param.AsDouble() * UnitConverter.HostToDynamoFactor(
                            ParameterTypeToUnitType(paramType));
                    else
                        result = param.AsDouble();
                    break;
                default:
                    throw new Exception(string.Format(Properties.Resources.ParameterWithoutStorageType, param));
            }

            return result;
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
            patternCollector.OfClass(typeof(FillPatternElement));
            FillPatternElement solidFill = patternCollector.ToElements().Cast<FillPatternElement>().First(x => x.GetFillPattern().IsSolidFill);

            var overrideColor = new Autodesk.Revit.DB.Color(color.Red, color.Green, color.Blue);
            ogs.SetProjectionFillColor(overrideColor);
            ogs.SetProjectionFillPatternId(solidFill.Id);
            ogs.SetProjectionLineColor(overrideColor);
            view.SetElementOverrides(InternalElementId, ogs);

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
            var param = InternalElement.Parameters.Cast<Autodesk.Revit.DB.Parameter>().FirstOrDefault(x => x.Definition.Name == parameterName);

            if (param == null)
                throw new Exception(Properties.Resources.ParameterNotFound);

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            var dynval = value as dynamic;
            SetParameterValue(param, dynval);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #region dynamic parameter setting methods

        private static void SetParameterValue(Autodesk.Revit.DB.Parameter param, double value)
        {
            if (param.StorageType != StorageType.Integer && param.StorageType != StorageType.Double)
                throw new Exception(Properties.Resources.ParameterStorageNotNumber);

            var valueToSet = GetConvertedParameterValue(param, value);
            
            param.Set(valueToSet);
        }

        private static void SetParameterValue(Autodesk.Revit.DB.Parameter param, Element value)
        {
            if (param.StorageType != StorageType.ElementId)
                throw new Exception(Properties.Resources.ParameterStorageNotElement);

            param.Set(value.InternalElementId);
        }

        private static void SetParameterValue(Autodesk.Revit.DB.Parameter param, int value)
        {
            if (param.StorageType != StorageType.Integer && param.StorageType != StorageType.Double)
                throw new Exception(Properties.Resources.ParameterStorageNotNumber);

            var valueToSet = GetConvertedParameterValue(param, value);

            param.Set(valueToSet);
        }

        private static void SetParameterValue(Autodesk.Revit.DB.Parameter param, string value)
        {
            if (param.StorageType != StorageType.String)
                throw new Exception(Properties.Resources.ParameterStorageNotString);

            param.Set(value);
        }

        private static void SetParameterValue(Autodesk.Revit.DB.Parameter param, bool value)
        {
            if (param.StorageType != StorageType.Integer)
                throw new Exception(Properties.Resources.ParameterStorageNotInteger);

            param.Set(value == false ? 0 : 1);
        }

        private static double GetConvertedParameterValue(Autodesk.Revit.DB.Parameter param, double value)
        {
            var paramType = param.Definition.ParameterType;

            if (IsConvertableParameterType(paramType))
            {
                return value * UnitConverter.DynamoToHostFactor(ParameterTypeToUnitType(paramType));
            }

            return value;
        }

        #endregion

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

        #region Location extraction & manipulation

        /// <summary>
        /// Update an existing element's location
        /// </summary>
        /// <param name="geometry">New Location Point or Curve</param>
        public void SetLocation(Geometry geometry)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);

            if (this.InternalElement.Location.GetType() == typeof(Autodesk.Revit.DB.LocationPoint))
            {
                if (geometry.GetType() == typeof(Autodesk.DesignScript.Geometry.Point))
                {
                    Autodesk.DesignScript.Geometry.Point point = (Autodesk.DesignScript.Geometry.Point)geometry;
                    Autodesk.Revit.DB.LocationPoint pt = (Autodesk.Revit.DB.LocationPoint)this.InternalElement.Location;
                    pt.Point = point.ToRevitType(true);
                }
                else
                    throw new Exception("Point element required");
            }
            else if (this.InternalElement.Location.GetType() == typeof(Autodesk.Revit.DB.LocationCurve) && geometry is Curve)
            {
                if (geometry.GetType() == typeof(Curve))
                {
                    Curve dynamoCurve = geometry as Curve;
                    Autodesk.Revit.DB.LocationCurve curve = (Autodesk.Revit.DB.LocationCurve)this.InternalElement.Location;
                    curve.Curve = dynamoCurve.ToRevitType(true);
                }
                else
                    throw new Exception("Curve element required");
            }
            else
            {
                throw new Exception("Element does not have a location that can be updated");
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get an exsiting element's location
        /// </summary>
        /// <returns>Location Geometry</returns>
        public Geometry GetLocation()
        {
            if (this.InternalElement.Location.GetType() == typeof(Autodesk.Revit.DB.LocationPoint))
            {
                Autodesk.Revit.DB.LocationPoint pt = (Autodesk.Revit.DB.LocationPoint)this.InternalElement.Location;
                return pt.Point.ToPoint(true);
            }
            else if (this.InternalElement.Location.GetType() == typeof(Autodesk.Revit.DB.LocationCurve))
            {
                Autodesk.Revit.DB.LocationCurve curve = (Autodesk.Revit.DB.LocationCurve)this.InternalElement.Location;
                return curve.Curve.ToProtoType(true);
            }
            else
            {
                throw new Exception("Element location not extractable");
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
                throw new Exception("Element could not be moved");
            }
        }


        #endregion

        #region Material

        /// <summary>
        /// Get Material Names from a Revit Element
        /// </summary>
        /// <param name="paintMaterials">Paint Materials</param>
        /// <returns>List of Names</returns>
        public List<Material> GetMaterials(bool paintMaterials = false)
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

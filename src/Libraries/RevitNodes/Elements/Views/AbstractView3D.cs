using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    [IsVisibleInDynamoLibrary(false)]
    [DynamoServices.RegisterForTrace]
    public abstract class AbstractView3D : View
    {
        [IsVisibleInDynamoLibrary(false)]
        public const string DefaultViewName = "dynamo3D";

        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.View3D InternalView3D
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalView3D; }
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Build Orientation3D object for eye point and a target point 
        /// </summary>
        /// <param name="eyePoint"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected static ViewOrientation3D BuildOrientation3D(XYZ eyePoint, XYZ target)
        {
            var globalUp = XYZ.BasisZ;
            var direction = target.Subtract(eyePoint).Normalize();

            // If the direction is zero length (ex. the eye and target
            // points are coincident) than set the direction to look
            // along the x axis by default. Otherwise, the call to
            // create a ViewOrientation3D object will fail.
            if (direction.IsZeroLength())
            {
                direction = XYZ.BasisX;
            }

            // If the direction points along Z, then 
            // switch the global up to Y
            if (direction.IsAlmostEqualTo(globalUp) ||
                (direction.Negate().IsAlmostEqualTo(globalUp)))
            {
                globalUp = XYZ.BasisY;
            }

            var up = direction.CrossProduct(globalUp).CrossProduct(direction);

            if (up.IsZeroLength())
            {
                up = XYZ.BasisZ;
            }

            // If up is zero length (ex. the direction vector is the z axis),
            // set the up to be along the Z axis.
            return new ViewOrientation3D(eyePoint, up, direction);

        }

        /// <summary>
        /// Obtain a sparse point collection outlining a Revit element bt traversing it's
        /// GeometryObject representation
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pts"></param>
        protected static void GetPointCloud(Autodesk.Revit.DB.Element e, List<XYZ> pts)
        {
            var options = new Options
            {
                ComputeReferences = true,
                DetailLevel = ViewDetailLevel.Coarse,
                IncludeNonVisibleObjects = false
            };

            foreach (var gObj in e.get_Geometry(options))
            {
                if (gObj is Solid)
                {
                    GetPointCloud(gObj as Solid, pts);
                }
                else if (gObj is GeometryInstance)
                {
                    GetPointCloud(gObj as GeometryInstance, pts);
                }
            }
        }

        /// <summary>
        /// Obtain a point collection outlining a GeometryObject
        /// </summary>
        /// <param name="geomInst"></param>
        /// <param name="pts"></param>
        protected static void GetPointCloud(GeometryInstance geomInst, List<XYZ> pts)
        {
            foreach (var gObj in geomInst.GetInstanceGeometry())
            {
                if (gObj is Solid)
                {
                    GetPointCloud(gObj as Solid, pts);
                }
                else if (gObj is GeometryInstance)
                {
                    GetPointCloud(gObj as GeometryInstance, pts);
                }
            }
        }

        /// <summary>
        /// Obtain a point collection outlining a Solid GeometryObject
        /// </summary>
        /// <param name="solid"></param>
        /// <param name="pts"></param>
        protected static void GetPointCloud(Solid solid, List<XYZ> pts)
        {
            foreach (Edge gEdge in solid.Edges)
            {
                var c = gEdge.AsCurve();
                if (c is Line)
                {
                    pts.Add(c.Evaluate(0, true));
                    pts.Add(c.Evaluate(1, true));
                }
                else
                {
                    IList<XYZ> xyzArray = gEdge.Tessellate();
                    pts.AddRange(xyzArray);
                }
            }
        }

        /// <summary>
        /// Make a single element appear in a particular view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="element"></param>
        protected static void IsolateInView(Autodesk.Revit.DB.View3D view, Autodesk.Revit.DB.Element element)
        {
            var fec = GetVisibleElementFilter();

            view.CropBoxActive = true;

            var toHide =
                fec.ToElements().Where(x => !x.IsHidden(view) && x.CanBeHidden(view) && x.Id != element.Id).Select(x => x.Id).ToList();

            if (toHide.Count > 0)
                view.HideElements(toHide);

            DocumentManager.Regenerate();

            // After a regeneration, we need to ensure that a 
            // transaction is re-opened.
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            if (view.IsPerspective)
            {
                var farClip = view.LookupParameter("Far Clip Active");
                farClip.Set(0);
            }
            else
            {
                var pts = new List<XYZ>();

                GetPointCloud(element, pts);

                var bounding = view.CropBox;
                var transInverse = bounding.Transform.Inverse;
                var transPts = pts.Select(transInverse.OfPoint);

                //ingore the Z coordindates and find
                //the max X ,Y and Min X, Y in 3d view.
                double dMaxX = 0, dMaxY = 0, dMinX = 0, dMinY = 0;

                var bFirstPt = true;
                foreach (var pt1 in transPts)
                {
                    if (bFirstPt)
                    {
                        dMaxX = pt1.X;
                        dMaxY = pt1.Y;
                        dMinX = pt1.X;
                        dMinY = pt1.Y;
                        bFirstPt = false;
                    }
                    else
                    {
                        if (dMaxX < pt1.X)
                            dMaxX = pt1.X;
                        if (dMaxY < pt1.Y)
                            dMaxY = pt1.Y;
                        if (dMinX > pt1.X)
                            dMinX = pt1.X;
                        if (dMinY > pt1.Y)
                            dMinY = pt1.Y;
                    }
                }

                bounding.Max = new XYZ(dMaxX, dMaxY, bounding.Max.Z);
                bounding.Min = new XYZ(dMinX, dMinY, bounding.Min.Z);
                view.CropBox = bounding;
            }

            view.CropBoxVisible = false;

        }

        /// <summary>
        /// Set the cropping for the current view
        /// </summary>
        /// <param name="view3D"></param>
        /// <param name="bbox"></param>
        private void IsolateInView(Autodesk.Revit.DB.View3D view3D, BoundingBoxXYZ bbox)
        {
            view3D.CropBox = bbox;
        }

        /// <summary>
        /// Create a Revit 3D View
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="name"></param>
        /// <param name="isPerspective"></param>
        /// <returns></returns>
        protected static Autodesk.Revit.DB.View3D Create3DView(ViewOrientation3D orient, string name, bool isPerspective)
        {
            // (sic) From the Dynamo legacy implementation
            var viewFam = DocumentManager.Instance.ElementsOfType<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            if (viewFam == null)
            {
                throw new Exception("There is no three dimensional view family int he document");
            }

            var view = isPerspective 
                ? Autodesk.Revit.DB.View3D.CreatePerspective(Document, viewFam.Id) 
                : Autodesk.Revit.DB.View3D.CreateIsometric(Document, viewFam.Id);

            view.SetOrientation(orient);
            view.SaveOrientationAndLock();
            view.Name = CreateUniqueViewName(name);

            return view;
        }

        /// <summary>
        /// Determines whether a view with the provided name already exists.
        /// If a view exists with the provided name, and new view is created with
        /// a unique name. Otherwise, the original view name is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The original name if it is already unique, or 
        /// a unique version of the name.</returns>
        public static string CreateUniqueViewName(string name)
        {
            var collector = new FilteredElementCollector(Document);
            collector.OfClass(typeof(Autodesk.Revit.DB.View));

            // If the name is already unique then return it.
            if (collector.All(x => x.Name != name))
                return name;

            // Create a unique name by appending a guid to
            // the end of the view name
            var viewName = $"{name}_{Guid.NewGuid()}";

            return viewName;
        }

        /// <summary>
        /// Utility method to create a filtered element collector which collects all elements in a view
        /// which Dynamo would like to view or on which Dynamo would like to operate.
        /// </summary>
        /// <returns></returns>
        protected static FilteredElementCollector GetVisibleElementFilter()
        {
            var fec = new FilteredElementCollector(Document);
            var filterList = new List<ElementFilter>();

            var fContinuousRail = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.ContinuousRail));
            var fRailing = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.Railing));
            var fStairs = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.Stairs));
            var fStairsLanding = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.StairsLanding));
            var fTopographySurface = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.TopographySurface));
            var fAssemblyInstance = new ElementClassFilter(typeof(AssemblyInstance));
            var fBaseArray = new ElementClassFilter(typeof(BaseArray));
            var fBeamSystem = new ElementClassFilter(typeof(BeamSystem));
            var fBoundaryConditions = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.BoundaryConditions));
            var fConnectorElement = new ElementClassFilter(typeof(ConnectorElement));
            var fControl = new ElementClassFilter(typeof(Control));
            var fCurveElement = new ElementClassFilter(typeof(Autodesk.Revit.DB.CurveElement));
            var fDividedSurface = new ElementClassFilter(typeof(Autodesk.Revit.DB.DividedSurface));
            var fCableTrayConduitRunBase = new ElementClassFilter(typeof(Autodesk.Revit.DB.Electrical.CableTrayConduitRunBase));
            var fHostObject = new ElementClassFilter(typeof(HostObject));
            var fInstance = new ElementClassFilter(typeof(Instance));
            var fmepSystem = new ElementClassFilter(typeof(MEPSystem));
            var fModelText = new ElementClassFilter(typeof(Autodesk.Revit.DB.ModelText));
            var fOpening = new ElementClassFilter(typeof(Opening));
            var fPart = new ElementClassFilter(typeof(Part));
            var fPartMaker = new ElementClassFilter(typeof(PartMaker));
            var fReferencePlane = new ElementClassFilter(typeof(Autodesk.Revit.DB.ReferencePlane));
            var fReferencePoint = new ElementClassFilter(typeof(Autodesk.Revit.DB.ReferencePoint));
            var fSpatialElement = new ElementClassFilter(typeof(SpatialElement));
            var fAreaReinforcement = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.AreaReinforcement));
            var fHub = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.Hub));
            var fPathReinforcement = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.PathReinforcement));
            var fRebar = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.Rebar));
            var fTruss = new ElementClassFilter(typeof(Autodesk.Revit.DB.Structure.Truss));

            #region Unused
            //Autodesk.Revit.DB.Analysis.AnalysisDisplayLegend;
            //Autodesk.Revit.DB.Analysis.AnalysisDisplayStyle;
            //Autodesk.Revit.DB.Analysis.MassEnergyAnalyticalModel;
            //Autodesk.Revit.DB.Analysis.MassLevelData;
            //Autodesk.Revit.DB.Analysis.MassSurfaceData;
            //Autodesk.Revit.DB.Analysis.MassZone;
            //Autodesk.Revit.DB.Analysis.SpatialFieldManager;
            //Autodesk.Revit.DB.AreaScheme;
            //Autodesk.Revit.DB.AppearanceAssetElement;
            //var FStairsPath = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.StairsPath));
            //var FStairsRun = new ElementClassFilter(typeof(Autodesk.Revit.DB.Architecture.StairsRun));
            //Autodesk.Revit.DB.AreaScheme;
            //ElementClassFilter FBasePoint = new ElementClassFilter(typeof(Autodesk.Revit.DB.BasePoint));
            //ElementClassFilter FCombinableElement = new ElementClassFilter(typeof(Autodesk.Revit.DB.CombinableElement));
            //Autodesk.Revit.DB..::..ComponentRepeater
            //Autodesk.Revit.DB..::..ComponentRepeaterSlot
            //Autodesk.Revit.DB.DesignOption;
            //Autodesk.Revit.DB.Dimension;
            //Autodesk.Revit.DB..::..DisplacementElement
            //Autodesk.Revit.DB.Electrical.ElectricalDemandFactorDefinition;
            //Autodesk.Revit.DB.Electrical.ElectricalLoadClassification;
            //Autodesk.Revit.DB.Electrical.PanelScheduleSheetInstance;
            //Autodesk.Revit.DB.Electrical.PanelScheduleTemplate;
            //var fElementType = new ElementClassFilter(typeof(ElementType));
            //Autodesk.Revit.DB..::..ElevationMarker
            //ElementClassFilter FFamilyBase = new ElementClassFilter(typeof(Autodesk.Revit.DB.FamilyBase));
            //Autodesk.Revit.DB.FilledRegion;
            //Autodesk.Revit.DB.FillPatternElement;
            //Autodesk.Revit.DB.FilterElement;
            //Autodesk.Revit.DB.GraphicsStyle;
            //Autodesk.Revit.DB.Grid;
            //ElementClassFilter FGroup = new ElementClassFilter(typeof(Autodesk.Revit.DB.Group));
            //Autodesk.Revit.DB.IndependentTag;
            //Autodesk.Revit.DB.Level;
            //Autodesk.Revit.DB.LinePatternElement;
            //Autodesk.Revit.DB.Material;
            //Autodesk.Revit.DB.Mechanical.Zone;
            //Autodesk.Revit.DB..::..MultiReferenceAnnotation
            //Autodesk.Revit.DB.Phase;
            //Autodesk.Revit.DB..::..PhaseFilter
            //Autodesk.Revit.DB.PrintSetting;
            //Autodesk.Revit.DB.ProjectInfo;
            //Autodesk.Revit.DB.PropertyLine;
            //ElementClassFilter FPropertySetElement = new ElementClassFilter(typeof(Autodesk.Revit.DB.PropertySetElement));
            //Autodesk.Revit.DB.PropertySetLibrary;
            //Autodesk.Revit.DB..::..ScheduleSheetInstance
            //Autodesk.Revit.DB..::..Segment
            //ElementClassFilter FSketchBase = new ElementClassFilter(typeof(Autodesk.Revit.DB.SketchBase));
            //ElementClassFilter FSketchPlane = new ElementClassFilter(typeof(Autodesk.Revit.DB.SketchPlane));
            //Autodesk.Revit.DB..::..SpatialElementCalculationLocation
            //ElementClassFilter FSpatialElementTag = new ElementClassFilter(typeof(Autodesk.Revit.DB.SpatialElementTag));
            //Autodesk.Revit.DB.Structure..::..AnalyticalLink
            //Autodesk.Revit.DB.Structure.AnalyticalModel;
            //Autodesk.Revit.DB.Structure..::..FabricArea
            //Autodesk.Revit.DB.Structure..::..FabricReinSpanSymbolControl
            //Autodesk.Revit.DB.Structure..::..FabricSheet
            //Autodesk.Revit.DB.Structure.LoadBase;
            //Autodesk.Revit.DB.Structure.LoadCase;
            //Autodesk.Revit.DB.Structure.LoadCombination;
            //Autodesk.Revit.DB.Structure.LoadNature;
            //Autodesk.Revit.DB.Structure.LoadUsage;
            //Autodesk.Revit.DB.Structure..::..RebarInSystem
            //Autodesk.Revit.DB.SunAndShadowSettings;
            //Autodesk.Revit.DB.TextElement;
            //Autodesk.Revit.DB.View;
            //Autodesk.Revit.DB..::..Viewport
            //Autodesk.Revit.DB.ViewSheetSet;
            //Autodesk.Revit.DB.WorksharingDisplaySettings;
            #endregion

            filterList.Add(fContinuousRail);
            filterList.Add(fRailing);
            filterList.Add(fStairs);
            filterList.Add(fStairsLanding);
            filterList.Add(fTopographySurface);
            filterList.Add(fAssemblyInstance);
            filterList.Add(fBaseArray);
            filterList.Add(fBeamSystem);
            filterList.Add(fBoundaryConditions);
            filterList.Add(fConnectorElement);
            filterList.Add(fControl);
            filterList.Add(fCurveElement);
            filterList.Add(fDividedSurface);
            filterList.Add(fCableTrayConduitRunBase);
            filterList.Add(fHostObject);
            filterList.Add(fInstance);
            filterList.Add(fmepSystem);
            filterList.Add(fModelText);
            filterList.Add(fOpening);
            filterList.Add(fPart);
            filterList.Add(fPartMaker);
            filterList.Add(fReferencePlane);
            filterList.Add(fReferencePoint);
            filterList.Add(fAreaReinforcement);
            filterList.Add(fHub);
            filterList.Add(fPathReinforcement);
            filterList.Add(fRebar);
            filterList.Add(fTruss);
            filterList.Add(fSpatialElement);

            var cRvtLinks = new ElementCategoryFilter(BuiltInCategory.OST_RvtLinks);
            filterList.Add(cRvtLinks);

            var filters = new LogicalOrFilter(filterList);
            fec.WherePasses(filters).WhereElementIsNotElementType();

            return fec;
        }

        #endregion

        #region Protected mutators

        /// <summary>
        /// Set the name of the current view
        /// </summary>
        /// <param name="name"></param>
        protected void InternalSetName(string name)
        {
            if (name == DefaultViewName && InternalView3D.Name.Contains(DefaultViewName + "_"))
            {
                // Assume that this has already been set unique.
                return;
            }

            if (!InternalView3D.Name.Equals(name))
                InternalView3D.Name = CreateUniqueViewName(name);
        }

        /// <summary>
        /// Set the orientation of the view
        /// </summary>
        /// <param name="orient"></param>
        protected void InternalSetOrientation(ViewOrientation3D orient)
        {
            if (InternalView3D.ViewDirection.IsAlmostEqualTo(-orient.ForwardDirection) &&
                InternalView3D.Origin.IsAlmostEqualTo(orient.EyePosition)) return;

            InternalView3D.Unlock();
            InternalView3D.SetOrientation(orient);
            InternalView3D.SaveOrientationAndLock();
        }

        /// <summary>
        /// Isolate the element in the current view by creating a mininum size crop box around it
        /// </summary>
        /// <param name="element"></param>
        protected void InternalIsolateInView(Autodesk.Revit.DB.Element element)
        {
            IsolateInView(InternalView3D, element);
        }

        /// <summary>
        /// Isolate the bounding box in the current view
        /// </summary>
        /// <param name="bbox"></param>
        protected void InternalIsolateInView(BoundingBoxXYZ bbox)
        {
            IsolateInView(InternalView3D, bbox);
        }

        /// <summary>
        /// Show all hiddent elements in the view
        /// </summary>
        protected void InternalRemoveIsolation()
        {
            InternalView3D.UnhideElements(GetVisibleElementFilter().ToElementIds());
            InternalView3D.CropBoxActive = false;
        }

        /// <summary>
        /// Set the InternalView3D property and the associated element id and unique id
        /// </summary>
        /// <param name="view"></param>
        protected void InternalSetView3D(Autodesk.Revit.DB.View3D view)
        {
            InternalView3D = view;
            InternalElementId = view.Id;
            InternalUniqueId = view.UniqueId;
        }

        #endregion
    }
}

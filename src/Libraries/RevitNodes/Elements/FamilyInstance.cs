using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;
using DynamoUnits;
using Revit.Elements.InternalUtilities;
using Revit.GeometryReferences;
using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyInstance
    /// </summary>
    [RegisterForTrace]
    public class FamilyInstance : AbstractFamilyInstance
    {

        #region Private constructors

        /// <summary>
        /// Wrap an existing FamilyInstance.
        /// </summary>
        /// <param name="instance"></param>
        protected FamilyInstance(Autodesk.Revit.DB.FamilyInstance instance)
        {
            SafeInit(() => InitFamilyInstance(instance));
        }

        /// <summary>
        /// Internal constructor for a FamilyInstance
        /// </summary>
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos,
            Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitFamilyInstance(fs, pos, level));
        }

        /// <summary>
        /// Internal constructor for a FamilyInstance
        /// </summary>
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos)
        {
            SafeInit(() => InitFamilyInstance(fs, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Reference reference, Line pos)
        {
            SafeInit(() => InitFamilyInstance(fs, reference, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Reference reference, XYZ location, XYZ referenceDirection)
        {
            SafeInit(() => InitFamilyInstance(fs, reference, location, referenceDirection));
        }
        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        /// <param name="instance"></param>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilyInstance instance)
        {
            InternalSetFamilyInstance(instance);
        }

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos,
            Autodesk.Revit.DB.Level level)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetLevel(level);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(pos, fs, level,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(
                    pos, fs, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(pos, fs,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(
                    pos, fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(FamilySymbol fs, Reference reference, Line pos)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was an existing family instance, rebind to that, and adjust its position
            if (oldFam != null && oldFam.HostFace.ElementId == reference.ElementId)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(reference, pos, fs);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(reference, pos, fs);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(FamilySymbol fs, Reference reference, XYZ location,
            XYZ referenceDirection)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was an existing family instance, rebind to that, and adjust its position
            if (oldFam != null && oldFam.HostFace.ElementId == reference.ElementId)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(location);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(reference,  location, referenceDirection, fs);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(reference, location, referenceDirection, fs);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }
        #endregion

        #region Private mutators

        private void InternalSetLevel(Autodesk.Revit.DB.Level level)
        {
            if (InternalFamilyInstance.LevelId.Compare(level.Id) == 0)
                return;

            TransactionManager.Instance.EnsureInTransaction(Document);

            // http://thebuildingcoder.typepad.com/blog/2011/01/family-instance-missing-level-property.html
            InternalFamilyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level.Id);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(XYZ fi)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as LocationPoint;
            if (lp != null && !lp.Point.IsAlmostEqualTo(fi)) lp.Point = fi;

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(Curve pos)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as LocationCurve;

            if (lp != null && lp.Curve != pos) lp.Curve = pos;

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets family type of the specific family instance
        /// </summary>
        /// <search>
        /// symbol
        /// </search>
        public new FamilyType Type
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Type; }
        }
        /// <summary>
        /// Gets the location of the specific family instance 
        /// </summary>
        public new Point Location
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Location; }
        }

        /// <summary>
        /// Gets the FacingOrientation of the family instance
        /// </summary>
        public Vector FacingOrientation
        {
            get
            {
                return InternalFamilyInstance.IsValidObject ? 
                    InternalFamilyInstance.FacingOrientation.ToVector() : null;
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinates in world space
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static FamilyInstance ByPoint(FamilyType familyType, Point point)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            } 
            
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }

            return new FamilyInstance(familyType.InternalFamilySymbol, point.ToXyz());
        }

        /// <summary>
        /// Place a Revit family instance of the given the FamilyType (also known as the FamilySymbol in the Revit API) 
        /// on a surface derived from a backing Revit face as reference and a line as reference for its position.
        /// 
        /// Note: The FamilyPlacementType must be CurveBased and the input surface must be created from a Revit Face 
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="face">Surface geometry derived from a Revit face as reference element</param>
        /// <param name="line">A line on the face defining where the symbol is to be placed</param>
        /// <returns>FamilyInstance</returns>
        public static FamilyInstance ByFace(FamilyType familyType, Surface face, Autodesk.DesignScript.Geometry.Line line)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }
            if (face == null)
            {
                throw new ArgumentNullException("face");
            }
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            var reference = ElementFaceReference.TryGetFaceReference(face);

            return new FamilyInstance(familyType.InternalFamilySymbol, reference.InternalReference, (Line) line.ToRevitType());
        }

        /// <summary>
        /// Place a Revit family instance given the FamilyType (also known as the FamilySymbol in the Revit API) 
        /// on a surface derived from a backing Revit face as reference, a reference direction and a point location where to place the family.
        /// 
        /// Note: The FamilyType should be workplane based and the input surface must be created from a Revit Face. The reference direction defines the rotation of the instance on the reference, and thus cannot be perpendicular to the face.
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="face">Surface geometry derived from a Revit face as reference element</param>
        /// <param name="location">Point on the face where the instance is to be placed</param>
        /// <param name="referenceDirection">A vector that defines the direction of placement of the family instance</param>
        /// <returns>FamilyInstance</returns>
        public static FamilyInstance ByFace(FamilyType familyType, Surface face, Point location, 
            Vector referenceDirection)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }
            if (face == null)
            {
                throw new ArgumentNullException("face");
            }
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if (referenceDirection == null)
            {
                throw new ArgumentNullException("referenceDirection");
            }
            var reference = ElementFaceReference.TryGetFaceReference(face);

            return new FamilyInstance(familyType.InternalFamilySymbol, reference.InternalReference, 
                location.ToXyz(), referenceDirection.ToXyz());
        }

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinates in world space
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="x">X coordinate in meters</param>
        /// <param name="y">Y coordinate in meters</param>
        /// <param name="z">Z coordinate in meters</param>
        /// <returns></returns>
        public static FamilyInstance ByCoordinates(FamilyType familyType, double x = 0, double y = 0, double z = 0)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var pt = Point.ByCoordinates(x, y, z);

            return ByPoint(familyType, pt);
        }

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API), it's coordinates in world space, and the Level
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="point">Point in meters</param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static FamilyInstance ByPointAndLevel(FamilyType familyType, Point point, Level level)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            return new FamilyInstance(familyType.InternalFamilySymbol, point.ToXyz(), level.InternalLevel);
        }

        /// <summary>
        /// Obtain a collection of FamilyInstances from the Revit Document and use them in the Dynamo graph
        /// </summary>
        /// <param name="familyType"></param>
        /// <returns></returns>
        /// <search>
        /// byfamilysymbol,ByFamilySymbol
        /// </search>
        public static FamilyInstance[] ByFamilyType(FamilyType familyType)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            return DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.FamilyInstance>()
                .Where(x => x.Symbol.Id == familyType.InternalFamilySymbol.Id)
                .Select(x => FromExisting(x, true))
                .ToArray();
        }

        /// <summary>
        /// Get Solid from Element helper method
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Solid GetSolidFromElement(Autodesk.Revit.DB.Element element)
        {
            GeometryElement geo = element.get_Geometry(new Options() { ComputeReferences = true });
            var enumerator = geo.GetEnumerator();
            enumerator.MoveNext();
            GeometryInstance i = (GeometryInstance)enumerator.Current;
            var geom2 = i.GetInstanceGeometry();
            var enumarator2 = geom2.GetEnumerator();
            enumarator2.MoveNext();
            return (Solid)enumarator2.Current;
        }

        /// <summary>
        /// Set Family Parameter Helper method
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        private static void SetFamilyParameter(Autodesk.Revit.DB.Element element, BuiltInParameter parameter, object value)
        {
            Autodesk.Revit.DB.Parameter p = element.get_Parameter(parameter);
            if (p != null && !p.IsReadOnly)
            {
                if (value.GetType() == typeof(ElementId)){ p.Set((ElementId)value); }
                if (value.GetType() == typeof(double)) { p.Set((double)value); }
                if (value.GetType() == typeof(int)) { p.Set((int)value); }
                if (value.GetType() == typeof(string)) { p.Set((string)value); }
            }
        }

        /// <summary>
        /// Create new Family Instance from Geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="templatePath"></param>
        /// <param name="isVoid"></param>
        /// <param name="subcategory"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static FamilyInstance ByGeometry(Autodesk.DesignScript.Geometry.Geometry geometry, string name, Category category, string templatePath, bool isVoid = false, string subcategory = "", string material = "")
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.ForceCloseTransaction();

            // create a temp sat file
            string tempFile = System.IO.Path.GetTempFileName() + ".sat";

            // create a temp family file
            string tempDir = System.IO.Path.GetTempPath();
            string tempFamilyFile = tempDir +"\\"+ name + ".rfa";

            // Get the conversion factor based on the display units
            DisplayUnitType units = document.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            double factor = UnitUtils.ConvertToInternalUnits(1, units);

            // scale the incoming geometry
            geometry = geometry.Scale(factor);

            // get a displacement vector
            Vector vector = Vector.ByTwoPoints(Autodesk.DesignScript.Geometry.BoundingBox.ByGeometry(geometry).MinPoint, Autodesk.DesignScript.Geometry.Point.Origin());
            
            // translate the geometry to origin
            geometry = geometry.Translate(vector);

            // export geometry to SAT
            geometry.ExportToSAT(tempFile);

            // create a new family document using the supplied template
            Autodesk.Revit.DB.Document familyDocument = document.Application.NewFamilyDocument(templatePath);

            // Get the families 3d view
            var collector = new Autodesk.Revit.DB.FilteredElementCollector(familyDocument).OfClass(typeof(Autodesk.Revit.DB.View));
            Autodesk.Revit.DB.View view = null;
            foreach (Autodesk.Revit.DB.View v in collector.ToElements())
            {
                if (!v.IsTemplate && v.ViewType == ViewType.ThreeD)
                { 
                    view = v;
                }
            }

            // Import the sat file to origin in feet
            TransactionManager.Instance.EnsureInTransaction(familyDocument);
            ElementId importedElementId = familyDocument.Import(tempFile, new SATImportOptions() { Placement = ImportPlacement.Origin, Unit = ImportUnit.Foot }, view);

            // get the solid element from the imported sat file
            Solid solid = GetSolidFromElement(familyDocument.GetElement(importedElementId));
            
            // delete imported sat
            familyDocument.Delete(importedElementId);
            System.IO.File.Delete(tempFile);

            // Set the families category
            familyDocument.OwnerFamily.FamilyCategory = familyDocument.Settings.Categories.get_Item(category.Name);

            // create a free form element from the solid geometry
            FreeFormElement freeFormElement = FreeFormElement.Create(familyDocument, solid);

            // if the geometry should be void set parameters accordingly
            if (isVoid)
            {
                SetFamilyParameter(freeFormElement, BuiltInParameter.ELEMENT_IS_CUTTING, 1);
                SetFamilyParameter(freeFormElement, BuiltInParameter.FAMILY_ALLOW_CUT_WITH_VOIDS, 1);
            }
            else
            {
                // Apply material if supplied
                if (material != string.Empty)
                {
                    Autodesk.Revit.DB.FilteredElementCollector materialCollector = new Autodesk.Revit.DB.FilteredElementCollector(familyDocument).OfClass(typeof(Autodesk.Revit.DB.Material));
                    foreach (Autodesk.Revit.DB.Material mat in materialCollector)
                    {
                        if (mat.Name == material)
                        {
                            SetFamilyParameter(freeFormElement, BuiltInParameter.MATERIAL_ID_PARAM, mat.Id);
                        }
                    }
                }

                // Apply Subcategory if supplied
                if (subcategory != string.Empty)
                {
                    var current_fam_cat = familyDocument.OwnerFamily.FamilyCategory;
                    var new_subcat = familyDocument.Settings.Categories.NewSubcategory(current_fam_cat, subcategory);
                    freeFormElement.Subcategory = new_subcat;
                }
            }

            TransactionManager.Instance.ForceCloseTransaction();

            // Save Family document and load it into the project
            familyDocument.SaveAs(tempFamilyFile, new SaveAsOptions() { OverwriteExistingFile = true });
            var family = familyDocument.LoadFamily(document, new FamOpt1());
            
            // close and delete family
            familyDocument.Close(false);
            System.IO.File.Delete(tempFamilyFile);
            
            // get imported family symbol
            var symbols = family.GetFamilySymbolIds().GetEnumerator();
            symbols.MoveNext();
            FamilySymbol symbol1 = (FamilySymbol)document.GetElement(symbols.Current);

            // activate symbol
            TransactionManager.Instance.EnsureInTransaction(document);
            if (!symbol1.IsActive) symbol1.Activate();

            // place instance in  correct location
            Autodesk.Revit.DB.FamilyInstance instance = document.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbol1, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            ElementTransformUtils.MoveElement(document, instance.Id, vector.Reverse().ToXyz());

            TransactionManager.Instance.ForceCloseTransaction();
            return new FamilyInstance(instance);
        }

        #endregion

        #region Internal static constructors 

        /// <summary>
        /// Construct a FamilyInstance from the Revit document. 
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FamilyInstance FromExisting(Autodesk.Revit.DB.FamilyInstance familyInstance, bool isRevitOwned)
        {
            return new FamilyInstance(familyInstance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.Name : "empty";
        }

        /// <summary>
        /// Gets the host of this fmaily instance (if any). Eg. returns the wall of a window or door family instance.
        /// </summary>
        public Element GetHost
        {
            get
            {
                if (this.InternalFamilyInstance.Host != null)

                    // TODO: This might need to change since its not clear if the Host element is revit owned or not.
                    // Currently there is no way of figuring this out, therefore the assumption is true (Revit owned)
                    return ElementWrapper.Wrap(this.InternalFamilyInstance.Host, true);
                else
                    throw new Exception(Properties.Resources.InvalidHost);
            }
        }

        /// <summary>
        /// Gets the family of this family instance
        /// </summary>
        public Family GetFamily 
        { 
            get 
            { 
                return Family.FromExisting(this.InternalFamilyInstance.Symbol.Family, true);
            }
        }

        /// <summary>
        /// Gets the family type of this family instance
        /// </summary>
        public FamilyType GetType
        { 
            get
            { 
                return FamilyType.FromExisting(this.InternalFamilyInstance.Symbol, true);
            }
        }

        

        /// <summary>
        /// Set the Euler angle of the family instance around its local Z-axis.
        /// </summary>        
        /// <param name="degree">The Euler angle around Z-axis.</param>
        /// <returns>The result family instance.</returns>
        public FamilyInstance SetRotation(double degree)
        {
            if (this == null)
            {
                throw new ArgumentNullException("familyInstance");
            }

            var oldTransform = InternalGetTransform();
            double[] oldRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(oldTransform, out oldRotationAngles);
            double newRotationAngle = degree * Math.PI / 180;

            if (!oldRotationAngles[0].AlmostEquals(newRotationAngle, 1.0e-6))
            {
                double rotateAngle = newRotationAngle - oldRotationAngles[0];
                var axis = Line.CreateUnbound(oldTransform.Origin, oldTransform.BasisZ);
                ElementTransformUtils.RotateElement(Document, new ElementId(Id), axis, -rotateAngle);
            }

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Get the transform of the internal family instance
        /// </summary>
        /// <returns>The internal transform</returns>
        private Transform InternalGetTransform()
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Document.Regenerate();

            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.GetTransform() : null;
        }

        #endregion
    }

    public class FamOpt1 : IFamilyLoadOptions
	{
        public FamOpt1(){}

        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Autodesk.Revit.DB.Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Project;
            overwriteParameterValues = true;
            return true;
        }

        }
}

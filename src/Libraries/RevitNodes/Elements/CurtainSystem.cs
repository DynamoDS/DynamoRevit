using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit CurtainSystem
    /// </summary>
    public class CurtainSystem : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit CurtainSystem
        /// </summary>
        internal Autodesk.Revit.DB.CurtainSystem InternalCurtainSystem
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalCurtainSystem; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private CurtainSystem(Autodesk.Revit.DB.CurtainSystem curtainSystem)
        {
            SafeInit(() => InitCurtainSystem(curtainSystem));
        }
      
        /// <summary>
        /// Private constructor
        /// </summary>
        private CurtainSystem(ReferenceArray faces, Autodesk.Revit.DB.CurtainSystemType curtainSystemType)
        {
            SafeInit(() => InitCurtainSystem(faces, curtainSystemType));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a CurtainSystem element
        /// </summary>
        private void InitCurtainSystem(Autodesk.Revit.DB.CurtainSystem curtainSystem)
        {
            InternalSetCurtainSystem(curtainSystem);
        }

        /// <summary>
        /// Initialize a CurtainSystem element
        /// </summary>
        private void InitCurtainSystem(ReferenceArray faces, Autodesk.Revit.DB.CurtainSystemType curtainSystemType)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.CurtainSystem curtainSystem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.CurtainSystem>(Document);

            ICollection<ElementId> ids = Document.Create.NewCurtainSystem2(faces, curtainSystemType);
            if (ids.Count > 1)
            {
                foreach (ElementId id in ids)
                {
                    Autodesk.Revit.DB.CurtainSystem system = (Autodesk.Revit.DB.CurtainSystem)Document.GetElement(id);
                    InitCurtainSystem(system);
                }
                return;
            }
            else
            {
                curtainSystem = (Autodesk.Revit.DB.CurtainSystem)Document.GetElement(ids.First());
            }

            InternalSetCurtainSystem(curtainSystem);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalCurtainSystem);
        }

        #endregion

        #region Private mutators


        /// <summary>
        /// Set the InternalCurtainSystem property and the associated element id and unique id
        /// </summary>
        /// <param name="curtainSystem"></param>
        private void InternalSetCurtainSystem(Autodesk.Revit.DB.CurtainSystem curtainSystem)
        {
            InternalCurtainSystem = curtainSystem;
            InternalElementId = curtainSystem.Id;
            InternalUniqueId = curtainSystem.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create Curtain System from face references
        /// </summary>
        /// <param name="face"></param>
        /// <param name="curtainSystemType"></param>
        /// <returns></returns>
        public static CurtainSystem ByFace(Autodesk.DesignScript.Geometry.Surface face, CurtainSystemType curtainSystemType)
        {
            ReferenceArray ca = new ReferenceArray();

            var reference = face.Tags.LookupTag("RevitFaceReference");
            if (reference == null) throw new Exception(Properties.Resources.FaceReferenceFailure);
            ca.Append((Autodesk.Revit.DB.Reference)reference);

            return new CurtainSystem(ca, curtainSystemType.InternalCurtainSystemType);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Add Curtain Grid
        /// </summary>
        /// <param name="face"></param>
        public void AddCurtainGrid(Autodesk.DesignScript.Geometry.Surface face)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            var reference = face.Tags.LookupTag("RevitFaceReference");
            if (reference != null)
            {
                this.InternalCurtainSystem.AddCurtainGrid((Autodesk.Revit.DB.Reference)reference);
            }
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Remove Curtain Grid
        /// </summary>
        /// <param name="face"></param>
        public void RemoveCurtainGrid(Autodesk.DesignScript.Geometry.Surface face)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            var reference = face.Tags.LookupTag("RevitFaceReference");
            if (reference != null)
            {
                this.InternalCurtainSystem.RemoveCurtainGrid((Autodesk.Revit.DB.Reference)reference);
            }
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get CurtainSystemType
        /// </summary>
        public CurtainSystemType CurtainSystemType
        {
            get
            {
                return (CurtainSystemType)this.InternalCurtainSystem.CurtainSystemType.ToDSType(true);
            }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a CurtainSystem from a user selected Element.
        /// </summary>
        /// <param name="curtainSystem"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static CurtainSystem FromExisting(Autodesk.Revit.DB.CurtainSystem curtainSystem, bool isRevitOwned)
        {
            return new CurtainSystem(curtainSystem)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}

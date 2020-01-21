using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;

namespace Revit.Elements
{
    public class Group : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit group
        /// </summary>
        internal Autodesk.Revit.DB.Group InternalGroup
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalGroup; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Group(Autodesk.Revit.DB.Group group)
        {
            SafeInit(() => InitGroup(group));
        }

        /// <summary>
        /// Initialize a group element
        /// </summary>
        private void InitGroup(Autodesk.Revit.DB.Group group)
        {
            InternalGroup = group;
            InternalElementId = group.Id;
            InternalUniqueId = group.UniqueId;
        }

        #endregion

        #region public static constructors

        /// <summary>
        /// Creates a new type of group, with all of the input elements.
        /// </summary>
        /// <param name="elements">Revit elements to group.</param>
        /// <returns>The new group type</returns>
        public static Group ByElements(List<Element> elements)
        {
            List<ElementId> elementIds = elements.Select(x => x.InternalElement.Id).ToList();
            TransactionManager.Instance.EnsureInTransaction(Document);
            var group = Document.Create.NewGroup(elementIds).ToDSType(true) as Group;
            TransactionManager.Instance.TransactionTaskDone();
            return group;
        }

        /// <summary>
        /// Place an instance of a Model Group into the Autodesk Revit document, using a location and a group type.
        /// </summary>
        /// <param name="location">The point to place the group.</param>
        /// <param name="groupType">The type of group to place.</param>
        /// <returns>The new group instance.</returns>
        public static Group PlaceInstance(Autodesk.DesignScript.Geometry.Point location, Element groupType)
        {
            var internalType = groupType.InternalElement as Autodesk.Revit.DB.GroupType;
            if (internalType == null)
                throw new InvalidOperationException(String.Format(Properties.Resources.InvalidGroupType, nameof(groupType)));

            XYZ newLocation = location.ToRevitType(true);
            TransactionManager.Instance.EnsureInTransaction(Document);
            Group newGroup = Document.Create.PlaceGroup(newLocation, internalType).ToDSType(true) as Group;
            TransactionManager.Instance.TransactionTaskDone();

            return newGroup;
        }

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point Location
        {
            get 
            {
                if (this.InternalGroup.IsAttached)
                    throw new InvalidOperationException(Properties.Resources.AttachedGroupLocation);

                var locationPt = this.InternalGroup.Location as LocationPoint;
                return locationPt.Point.ToPoint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Element GroupType
        {
            get { return this.InternalGroup.GroupType.ToDSType(true); }
        }

        /// <summary>
        /// Indicates wether or not this group is attached to a parent group.
        /// </summary>
        public bool IsAttached
        {
            get { return this.InternalGroup.IsAttached; }
        }

        /// <summary>
        /// Returns the attached detail groups available for this group.
        /// </summary>
        public List<Element> AttachedDetailGroup
        {
            get 
            {
                var attachedGroupIds = this.InternalGroup.GetAvailableAttachedDetailGroupTypeIds();
                return attachedGroupIds.Select(id => Document.GetElement(id).ToDSType(true)).ToList();
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Element> GetMembers()
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            List<ElementId> memberIds = this.InternalGroup.GetMemberIds().ToList();
            TransactionManager.Instance.TransactionTaskDone();
            return memberIds.Select(id => Document.GetElement(id).ToDSType(true)).ToList();         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Element> UngroupElements()
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            List<ElementId> ungroupedElementIds = this.InternalGroup.UngroupMembers().ToList();
            TransactionManager.Instance.TransactionTaskDone();
            return ungroupedElementIds.Select(id => Document.GetElement(id).ToDSType(true)).ToList();
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Group from a user selected Element.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Group FromExisting(Autodesk.Revit.DB.Group group, bool isRevitOwned)
        {
            return new Group(group)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}

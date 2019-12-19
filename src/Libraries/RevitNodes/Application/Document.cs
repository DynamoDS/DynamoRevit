
using Autodesk.Revit.DB;
using Revit.Elements;
using Revit.GeometryConversion;

using RevitServices.Persistence;
using System;
using View = Revit.Elements.Views.View;

namespace Revit.Application
{
    /// <summary>
    /// A Revit Document
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Internal reference to the Document
        /// </summary>
        internal Autodesk.Revit.DB.Document InternalDocument { get; private set; }

        internal Document(Autodesk.Revit.DB.Document currentDBDocument)
        {
            InternalDocument = currentDBDocument;  
        }

        /// <summary>
        /// Get the active view for the document
        /// </summary>
        public View ActiveView 
        {
            get
            {
                return (View)InternalDocument.ActiveView.ToDSType(true);
            }
        }

        /// <summary>
        /// Is the Document a Family?
        /// </summary>
        public bool IsFamilyDocument
        {
            get
            {
                return InternalDocument.IsFamilyDocument;
            }
        }

        /// <summary>
        /// The full path of the Document.
        /// </summary>
        public string FilePath
        {
            get { return InternalDocument.PathName ?? string.Empty; }
        }

        /// <summary>
        /// Get the current document
        /// </summary>
        /// <returns></returns>
        public static Document Current
        {
            get { return new Document(DocumentManager.Instance.CurrentDBDocument); }
        }

        /// <summary>
        /// Gets the worksharing path of the current document
        /// </summary>
        public string WorksharingPath
        {
            get
            {
                ModelPath modelPath = WorksharingModelPath;
                if (modelPath == null)
                    throw new NullReferenceException(Properties.Resources.DocumentNotWorkshared);
                return ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath); 
            }
        }

        /// <summary>
        /// Whether the Worksharing path represents a path on an Autodesk server such as BIM360.
        /// </summary>
        public bool IsCloudPath
        {
            get{ return WorksharingModelPath.CloudPath; }
        }

        internal ModelPath WorksharingModelPath
        {
            get { return this.InternalDocument.GetWorksharingCentralModelPath(); }
        }

        /// <summary>
        /// Extracts Latitude and Longitude from Revit
        /// </summary>
        /// 
        /// <returns name="Lat">Latitude</returns>
        /// <returns name="Long">Longitude</returns>
        /// <search>Latitude, Longitude</search>

        public DynamoUnits.Location Location
        {
            get
            {
                var loc = InternalDocument.SiteLocation;
                return DynamoUnits.Location.ByLatitudeAndLongitude(
                    loc.Latitude.ToDegrees(),
                    loc.Longitude.ToDegrees());
            }
        }
    }

}

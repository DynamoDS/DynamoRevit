﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit ImportInstance Element
    /// </summary>
    [RegisterForTrace]
    public class ImportInstance : Element
    {
        [Browsable(false)]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalImportInstance; }
        }

        internal Autodesk.Revit.DB.ImportInstance InternalImportInstance
        {
            get;
            private set;
        }

        #region Private constructor

        /// <summary>
        /// Constructor for ImportInstance
        /// </summary>
        /// <param name="satPath"></param>
        /// <param name="translation"></param>
        /// <param name="view"></param>
        internal ImportInstance(string satPath, XYZ translation = null, Revit.Elements.Views.View view = null)
        {
            SafeInit(() => InitImportInstance(satPath, view, translation));
        }

        /// <summary>
        /// ImportInstance from existing
        /// </summary>
        /// <param name="element"></param>
        private ImportInstance(Autodesk.Revit.DB.ImportInstance element)
        {
            SafeInit(() => InternalSetImportInstance(element),true);
        }

        #endregion

        /// <summary>
        /// Initialize an ImportInstance element
        /// </summary>
        /// <param name="satPath"></param>
        /// <param name="translation"></param>
        /// <param name="view"></param>
        private void InitImportInstance(string satPath, Revit.Elements.Views.View view, XYZ translation = null)
        {
            var instance = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ImportInstance>(Document);
            if (null != instance)
                DocumentManager.Instance.DeleteElement(new ElementUUID(instance.UniqueId));

            translation = translation ?? XYZ.Zero;

            TransactionManager.Instance.EnsureInTransaction(Document);

            var options = new SATImportOptions()
            {
                Unit = ImportUnit.Foot
            };

            var id = null != view ? Document.Import(satPath, options, view.InternalView) : Document.Import(satPath, options, Document.ActiveView);
            var element = Document.GetElement(id);
            var importInstance = element as Autodesk.Revit.DB.ImportInstance;

            if (importInstance == null)
            {
                throw new Exception(Properties.Resources.InstanceImportFailure);
            }

            InternalSetImportInstance(importInstance);
            InternalUnpinAndTranslateImportInstance(translation);

            this.Path = satPath;

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(importInstance);
        }

        private void InternalUnpinAndTranslateImportInstance(Autodesk.Revit.DB.XYZ translation)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // the element must be unpinned to translate
            InternalImportInstance.Pinned = false;

            if (!translation.IsZeroLength()) ElementTransformUtils.MoveElement(Document, InternalImportInstance.Id, translation);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetImportInstance(Autodesk.Revit.DB.ImportInstance ele)
        {
            this.InternalImportInstance = ele;
            this.InternalElementId = ele.Id;
            this.InternalUniqueId = ele.UniqueId;
        }

        #region Public properties
        /// <summary>
        /// Gets file path of the sat file that represents the geometry of the specified ImportInstance Element
        /// </summary>
        public string Path { get; private set; }

        #endregion

        /// <summary>
        /// Import Geometry from a SAT file.  The SAT file is assumed to be in Feet.
        /// </summary>
        /// <param name="pathToFile">The path to the SAT file</param>
        /// <returns></returns>
        public static ImportInstance BySATFile(string pathToFile)
        {

            if (pathToFile == null)
            {
                throw new ArgumentNullException("pathToFile");
            }

            if (!File.Exists(pathToFile))
            {
                throw new ArgumentException(string.Format(Properties.Resources.FileNotFound, pathToFile));
            }

            return new ImportInstance(pathToFile);
        }

        /// <summary>
        /// Import a collection of Geometry (Solid, Curve, Surface, etc) into Revit as an ImportInstance.  This variant is much faster than
        /// ImportInstance.ByGeometry as it uses a batch method.
        /// </summary>
        /// <param name="geometries">A collection of Geometry</param>
        /// <returns></returns>
        public static ImportInstance ByGeometries(Autodesk.DesignScript.Geometry.Geometry[] geometries)
        {
            if (geometries == null)
            {
                throw new ArgumentNullException("geometries");
            }

            var translation = Vector.ByCoordinates(0, 0, 0);

            string exported_fn = CreateSATFile(geometries, ref translation);

            return new ImportInstance(exported_fn, translation.ToXyz());
        }

        /// <summary>
        /// Import a collection of Geometry (Solid, Curve, Surface, etc) into Revit views as an ImportInstance.  This variant is much faster than
        /// ImportInstance.ByGeometry as it uses a batch method.
        /// </summary>
        /// <param name="geometries">A collection of Geometry</param>
        /// <param name="view">The view into which the ImportInstance will be imported.</param>
        /// <returns></returns>
        public static ImportInstance ByGeometriesAndView(Autodesk.DesignScript.Geometry.Geometry[] geometries, Revit.Elements.Views.View view)
        {
            if (geometries == null)
            {
                throw new ArgumentNullException("geometries");
            }

            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            
            var translation = Vector.ByCoordinates(0, 0, 0);
            
            var exported_fn = CreateSATFile(geometries, ref translation);

            return new ImportInstance(exported_fn, translation.ToXyz(), view);
        }

        /// <summary>
        /// Import a collection of Geometry (Solid, Curve, Surface, etc) into Revit as an ImportInstance.
        /// </summary>
        /// <param name="geometry">A single piece of geometry</param>
        /// <returns></returns>
        public static ImportInstance ByGeometry(Autodesk.DesignScript.Geometry.Geometry geometry)
        {
            List<Autodesk.DesignScript.Geometry.Geometry> geometries =
                new List<Autodesk.DesignScript.Geometry.Geometry>();
            geometries.Add(geometry);
            return ByGeometries(geometries.ToArray());
        }

        /// <summary>
        /// Import a collection of Geometry (Solid, Curve, Surface, etc) into Revit views as an ImportInstance.
        /// </summary>
        /// <param name="geometry">A single piece of geometry</param>
        /// <param name="view">The view into which the ImportInstance will be imported.</param>
        /// <returns></returns>
        public static ImportInstance ByGeometryAndView(Autodesk.DesignScript.Geometry.Geometry geometry, Revit.Elements.Views.View view)
        {
            List<Autodesk.DesignScript.Geometry.Geometry> geometries =
                new List<Autodesk.DesignScript.Geometry.Geometry>();
            geometries.Add(geometry);
            return ByGeometriesAndView(geometries.ToArray(), view);
        }

        #region Helper methods
        
        /// <summary>
        /// This method contains workarounds for increasing the robustness of input geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="translation"></param>
        private static void Robustify(ref Autodesk.DesignScript.Geometry.Geometry geometry,
            ref Autodesk.DesignScript.Geometry.Vector translation)
        {
            // translate centroid of the solid to the origin
            // export, then move back 
            if (geometry is Autodesk.DesignScript.Geometry.Solid)
            {
                var solid = geometry as Autodesk.DesignScript.Geometry.Solid;

                translation = solid.Centroid().AsVector();
                var tranGeo = solid.Translate(translation.Reverse());
                solid.Dispose();

                geometry = tranGeo;
            }
        }

        /// <summary>
        /// This method contains workarounds for increasing the robustness of input geometry
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="translation"></param>
        private static void Robustify(ref Autodesk.DesignScript.Geometry.Geometry[] geometries,
            ref Autodesk.DesignScript.Geometry.Vector translation)
        {
            // translate all geom to centroid of bbox, then translate back
            var bb = Autodesk.DesignScript.Geometry.BoundingBox.ByGeometry(geometries);

            // get center of bbox
            var trans = ((bb.MinPoint.ToXyz() + bb.MaxPoint.ToXyz())/2).ToVector().Reverse();
            bb.Dispose();

            // translate all geom so that it is centered by bb
            List<Geometry> newGeometries = new List<Geometry>();
            int count = geometries.Length;
            for (int i = 0; i < count; ++i)
            {
                var oldGeometry = geometries[i];
                geometries[i] = oldGeometry.Translate(trans);
                oldGeometry.Dispose();
            }

            // so that we can move it all back
            translation = trans.Reverse();
        }

        /// <summary>
        /// Create a SAT and export it to a temporary file.
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        private static string CreateSATFile(Autodesk.DesignScript.Geometry.Geometry[] geometries,
            ref Autodesk.DesignScript.Geometry.Vector translation)
        {
            // transform geometry from dynamo unit system (m) to revit (ft)
            var newGeometries = geometries.Select(x => x.InHostUnits()).ToArray();

            Robustify(ref newGeometries, ref translation);

            // Export to temporary file
            var fn = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".sat";
            var exported_fn = Autodesk.DesignScript.Geometry.Geometry.ExportToSAT(newGeometries, fn);

            newGeometries.ForEach(x => x.Dispose());

            return exported_fn;
        }

        #endregion

        internal static ImportInstance FromExisting(Autodesk.Revit.DB.ImportInstance instance, bool isRevitOwned)
        {
            return new ImportInstance(instance)
            {
                IsRevitOwned = isRevitOwned
            };
        }
    }
}

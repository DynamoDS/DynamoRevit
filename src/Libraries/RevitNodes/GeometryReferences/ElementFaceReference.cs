﻿using System;
using System.Linq;
using Autodesk.Revit.DB;
using Microsoft.CSharp.RuntimeBinder;

namespace Revit.GeometryReferences
{
    /// <summary>
    /// A stable reference to a Revit Face, usually derived from a Revit Element
    /// </summary>
    /// See: http://revitapisearch.com.s3-website-us-east-1.amazonaws.com/html/f3d5d2fe-96bf-8528-4628-78d8d5e6705f.htm
    public class ElementFaceReference : ElementGeometryReference
    {
        internal ElementFaceReference(Autodesk.Revit.DB.Face face)
        {
            if (face.Reference == null)
            {
                throw new Exception(Properties.Resources.FaceReferenceFailure);
            }
            this.InternalReference = face.Reference;
        }

        internal ElementFaceReference(Autodesk.Revit.DB.Reference reference)
        {
            this.InternalReference = reference;
        }

        internal static ElementFaceReference FromExisting(Autodesk.Revit.DB.Face arg)
        {
            return new ElementFaceReference(arg);
        }

        internal static Autodesk.DesignScript.Geometry.Surface AddTag( Autodesk.DesignScript.Geometry.Surface surface, Autodesk.Revit.DB.Reference reference )
        {
            surface.Tags.AddTag( DefaultTag, reference );
            return surface;
        }

        public const string DefaultTag = "RevitFaceReference";

        internal static ElementFaceReference TryGetFaceReference(object geometryObject, string nodeTypeString = "This node")
        {
            var geometry = (dynamic) geometryObject;

            try
            {
                return TryGetFaceReference(geometry);
            }
            catch (RuntimeBinderException)
            {
                throw new ArgumentException(string.Format(Properties.Resources.FaceReferenceExtractionFailure, nodeTypeString));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static ElementFaceReference TryGetFaceReference(ElementFaceReference curveObject)
        {
            return curveObject;
        }

        private static ElementFaceReference TryGetFaceReference(Revit.Elements.Element geometryObject, string nodeTypeString = "This node")
        {
            var cs = geometryObject.InternalGeometry().OfType<Autodesk.Revit.DB.Face>();
            if (cs.Any()) return new ElementFaceReference(cs.First());

            var ss = geometryObject.InternalGeometry().OfType<Autodesk.Revit.DB.Solid>();
            if (ss.Any()) return new ElementFaceReference(ss.First().Faces.Cast<Autodesk.Revit.DB.Face>().First());

            throw new ArgumentException(string.Format(Properties.Resources.FaceReferenceExtractionFailure, nodeTypeString) +
                string.Format(Properties.Resources.FaceReferenceExtractionDetail, geometryObject));
        }

        private static ElementFaceReference TryGetFaceReference(Autodesk.DesignScript.Geometry.Surface surfaceObject, string nodeTypeString = "This node")
        {
            // If a Reference has been added to this object, we can use that
            // to build the Element.
            object tagObj = surfaceObject.Tags.LookupTag(DefaultTag);
            if (tagObj != null)
            {
                var tagRef = (Reference)tagObj;
                return new ElementFaceReference(tagRef);
            }

            throw new ArgumentException(string.Format(Properties.Resources.FaceReferenceExtractionFailure, nodeTypeString) +
                string.Format(Properties.Resources.FaceReferenceHint, "ImportInstance.ByGeometry"));
        }

        /// <summary>
        /// Try get ElementFaceReference from a Revit Face.
        /// </summary>
        /// <param name="surface">A face from Revit</param>
        /// <returns></returns>
        public static ElementFaceReference BySurface(Autodesk.DesignScript.Geometry.Surface surface)
        {
            return TryGetFaceReference(surface);
        }
    }

}

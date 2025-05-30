﻿using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;

namespace Revit.GeometryObjects
{
    [IsVisibleInDynamoLibrary(false)]
    public static class GeometryObjectSelector
    {
        /// <summary>
        /// Return an AbstractGeometryObject given a string representation of the geometry's reference.
        /// </summary>
        /// <param name="referenceString"></param>
        /// <returns></returns>
        public static object ByReferenceStableRepresentation(string referenceString)
        {
            try
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                var elRef =
                    Reference.ParseFromStableRepresentation(doc, referenceString);

                var ele =
                    DocumentManager.Instance
                        .CurrentDBDocument.GetElement(elRef);

                var geob = ele.GetGeometryObjectFromReference(elRef);
                if (geob == null) return null;
                
                var familyInstance = ele as FamilyInstance;
                if (familyInstance != null && RequiresTransform(familyInstance))
                {
                    var transf = familyInstance.GetTransform().ToCoordinateSystem();
                    return geob.Convert(elRef, transf);
                }
                var importInstance = ele as ImportInstance;
                if (importInstance != null && RequiresTransform(importInstance))
                {
                    var transf = importInstance.GetTransform().ToCoordinateSystem();
                    return geob.Convert(elRef, transf);
                }

                return geob.Convert(elRef);
            }
            catch(Exception)
            {
                throw new Exception(Properties.Resources.GeometryObjectReferenceFailure);
            }
        }

        /// <summary>
        /// 
        ///     Determine if the Geometry extracted from a FamilyInstance requires transformation.
        /// 
        ///     Bizarrely, some FamilyInstance's geom is transformed and some not when obtained
        ///     from GetGeometryObjectFromReference.  This is because some need to be transformed
        ///     to interact with adjacent geometry in the document.  This stop-gap, suggested by
        ///     SC in the Revit API team, checks if there are any non-empty GeometryInstances in 
        ///     FamilyInstance's geometry.  Apparently this is a good heuristic for checking if
        ///     the geometry requires a transform or not.
        /// 
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        private static bool RequiresTransform(Autodesk.Revit.DB.FamilyInstance familyInstance)
        {
            if (familyInstance.Category.BuiltInCategory == BuiltInCategory.OST_Mass)
                return false;
            var geom = familyInstance.get_Geometry(new Options());
            return geom.OfType<Autodesk.Revit.DB.GeometryInstance>()
                .Any(x => x.GetInstanceGeometry().Any());
        }

        /// <summary>
        /// This is a workaround. Revit experts are positive about this.
        /// </summary>
        /// <param name="importInstance"></param>
        /// <returns></returns>
        private static bool RequiresTransform(Autodesk.Revit.DB.ImportInstance importInstance)
        {
            var geom = importInstance.get_Geometry(new Options());
            return geom.OfType<Autodesk.Revit.DB.GeometryInstance>()
                .Any(x => x.GetInstanceGeometry().Any());
        }

        public static GeometryReferences.ElementGeometryReference GetPointByReference(string referenceString)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var elRef =
                Reference.ParseFromStableRepresentation(doc, referenceString);

            if(elRef.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_LINEAR)
            {
                return new Revit.GeometryReferences.ElementCurveReference(elRef);
            }
            else if(elRef.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE)
            {
                return new Revit.GeometryReferences.ElementFaceReference(elRef);
            }
            else
            {
                throw new Exception(Properties.Resources.ReferenceSelectFailure);
            }
        }
    }
}

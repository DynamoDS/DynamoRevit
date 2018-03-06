using System;
using System.Reflection;
using Autodesk.DesignScript.Interfaces;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using Autodesk.DesignScript.Runtime;
namespace Revit.Elements
{
    public abstract class CurveElement : Element, IGraphicItem
    {
        private const byte DefR = 0;
        private const byte DefG = 0;
        private const byte DefB = 0;
        private const byte DefA = 255;

        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalCurveElement; }
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        internal Autodesk.Revit.DB.CurveElement InternalCurveElement
        {
            get;
            private set;
        }

        #region public properties

        /// <summary>
        /// Obtain the reference curve for this ModelCurve
        /// </summary>
        public ElementCurveReference ElementCurveReference
        {
            get
            {
                return new ElementCurveReference(InternalCurveElement.GeometryCurve);
            }
        }

        /// <summary>
        /// Obtain the geometry curve for this geometry curve
        /// </summary>
        public Autodesk.DesignScript.Geometry.Curve Curve
        {
            get
            {
                return InternalCurveElement.GeometryCurve.ToProtoType();
            }
        }

        #endregion

        #region protected mutators

        /// <summary>
        /// Set the geometry curve used by the ModelCurve
        /// </summary>
        /// <param name="c"></param>
        protected void InternalSetCurve(Curve c)
        {
            if (CurveUtils.CurvesAreSimilar(InternalCurveElement.GeometryCurve, c))
                return;

            if (!InternalCurveElement.GeometryCurve.IsBound && c.IsBound)
            {
                c = c.Clone();
                c.MakeUnbound();
            }
            setCurveMethod(this.InternalCurveElement, c);
        }

        /// <summary>
        /// Set the internal model curve along with its id's
        /// </summary>
        /// <param name="curveElement"></param>
        protected void InternalSetCurveElement(Autodesk.Revit.DB.CurveElement curveElement)
        {
            this.InternalCurveElement = curveElement;
            this.InternalElementId = curveElement.Id;
            this.InternalUniqueId = curveElement.UniqueId;
        }

        #endregion

        #region helper methods

        protected static bool hasMethodSetCurve = true;

        protected static void setCurveMethod(Autodesk.Revit.DB.CurveElement ce, Curve c)
        {
            bool foundMethod = false;

            if (hasMethodSetCurve)
            {
                Type CurveElementType = typeof(Autodesk.Revit.DB.CurveElement);
                MethodInfo[] curveElementMethods = CurveElementType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                String nameOfMethodSetCurve = "SetGeometryCurveOverridingJoins";

                foreach (MethodInfo m in curveElementMethods)
                {
                    if (m.Name == nameOfMethodSetCurve)
                    {
                        object[] argsM = new object[1];
                        argsM[0] = c;

                        foundMethod = true;
                        m.Invoke(ce, argsM);
                        break;
                    }
                }
            }
            if (!foundMethod)
            {
                hasMethodSetCurve = false;
                ce.GeometryCurve = c;
            }
        }

        #endregion

        /// <summary>
        /// Empty method - we don't want to tessellate curves automatically
        /// but it seems we need this method to correctly import this library into Dynamo.
        /// See description in base class.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public new void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            
        }

       private static byte[] CreateColorByteArrayOfSize(int size, byte red, byte green, byte blue, byte alpha)
       {
           var arr = new byte[size * 4];
           for (var i = 0; i < arr.Length; i += 4)
           {
               arr[i] = red;
               arr[i + 1] = green;
               arr[i + 2] = blue;
               arr[i + 3] = alpha;
           }
           return arr;
       }
    }
}

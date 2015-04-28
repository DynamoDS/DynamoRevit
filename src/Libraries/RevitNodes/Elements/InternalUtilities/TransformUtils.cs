using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using DynamoUnits;
using Autodesk.Revit.DB;
using System.Windows.Media.Media3D;

namespace Revit.Elements.InternalUtilities
{
   [IsVisibleInDynamoLibrary(false)]
   public class TransformUtils
   {
      // decomp_ZYX
      public static void ExtractEularAnglesFromTransform(Transform transform, out double[] angles )
      {
         angles = new double[3];
         
         double[][] trfVecs = new double[3][];
         double[] xVec = new double[3] { transform.BasisX.X, transform.BasisX.Y, transform.BasisX.Z };
         double[] yVec = new double[3] { transform.BasisY.X, transform.BasisY.Y, transform.BasisY.Z };
         double[] zVec = new double[3] { transform.BasisZ.X, transform.BasisZ.Y, transform.BasisZ.Z };
         trfVecs[0] = xVec;
         trfVecs[1] = yVec;
         trfVecs[2] = zVec;

         double cy = Math.Sqrt(trfVecs[0][0] * trfVecs[0][0] + trfVecs[1][0] * trfVecs[1][0]);
	      if(cy > 1.0e-6) 
         {
	         angles[0] = Math.Atan2(trfVecs[2][1], trfVecs[2][2]);
	         angles[1] = Math.Atan2(-trfVecs[2][0], cy);
	         angles[2] = Math.Atan2(trfVecs[1][0], trfVecs[0][0]);
	      }
         else
         {
	         angles[0] = Math.Atan2(-trfVecs[1][2], trfVecs[1][1]);
	         angles[1] = Math.Atan2(-trfVecs[2][0], cy);
	         angles[2] = 0.0;
	      }
         double t = angles[0];
         angles[0] = angles[2];
         angles[2] = t;
      }
   }
}

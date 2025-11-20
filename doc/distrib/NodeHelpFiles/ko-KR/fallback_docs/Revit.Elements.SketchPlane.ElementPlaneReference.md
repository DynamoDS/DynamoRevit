## 상세
This node retrieves the reference plane associated with a given sketch plane element. This helps in identifying or reusing the same reference plane for creating or modifying geometry.

In this example, a plane is defined then connected to the SketchPlane.ByPlane node, which generates a corresponding sketch plane.  This sketch plane is used as an input to SketchPlane.ElementPlaneReference where the out can then be used for dimensioning, alignment, constraints, or other operations that require a Revit reference.

___
## 예제 파일

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)

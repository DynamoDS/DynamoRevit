## En detalle:
This node retrieves the curve reference associated with a given Revit CurveElement, such as a model curve or detail line. The reference can then be used as input to other nodes that require a geometry reference—such as dimensioning, alignment, or divided path creation.

In this example, a model curve is created using a start point and end point, then input into CurveElement.ElementCurveReference node.  the output is a geometric reference of the curve element that can be used in downstream operations.
___
## Archivo de ejemplo

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

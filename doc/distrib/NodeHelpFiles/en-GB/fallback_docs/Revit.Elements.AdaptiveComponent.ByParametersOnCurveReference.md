## In Depth
This node places adaptive components by mapping parameter values along a selected curve to define placement positions for the adaptive family type.

In this example, a curve is drawn in the mass family and used for curve input. Parameter values are provided to position instances along the curve, and the "Diagnostic Tripod – 1 Point.rfa" family is chosen as the family type. The AdaptiveComponent.ByParametersOnCurveReference node outputs adaptive components placed along the selected curve.  Note that the "Diagnostic Tripod – 1 Point.rfa" needs to be loaded into your mass family before running this graph.

For this node help example file to run, you need to load "Diagnostics Tripod-1 point.rfa" into the Revit file. The family is stored here. C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data

___
## Example File

![AdaptiveComponent.ByParametersOnCurveReference](./Revit.Elements.AdaptiveComponent.ByParametersOnCurveReference_img.jpg)

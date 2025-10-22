## In Depth
This node places adaptive components by applying UV parameter values to a selected face, defining the placement locations for the adaptive family type.

In this example, a surface is created within the mass family by extruding a curve (this is done manually), and that surface is selected as the face input. UV values are then provided to determine placement positions, and the Diagnostic Tripod – 1 Point.rfa family is used as the type. The AdaptiveComponent.ByParametersOnFace node outputs adaptive components positioned on the selected face.  Note that the "Diagnostic Tripod – 1 Point.rfa" needs to be loaded into your mass family before running this graph.
___
## Example File

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)
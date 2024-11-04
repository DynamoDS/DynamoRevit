## In Depth
`View.SketchPlane` returns a sketch plane element for the given view. If no sketch plane is definied, null is returned. Sketch planes are referred to as "work plane" in Revit.

In the example below, the sketch plane for the active view is returned and converted into a Dynamo plane. The resulting Dynamo plane can be used for geometry creation.
___
## Example File

![View.SketchPlane](./Revit.Elements.Views.View.SketchPlane_img.jpg)
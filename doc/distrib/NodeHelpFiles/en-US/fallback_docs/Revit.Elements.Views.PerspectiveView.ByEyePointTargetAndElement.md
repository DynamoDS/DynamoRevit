## In Depth
This node creates a new Revit perspective 3D view using an eye point, target point, element, name and isolateElement.

In this example, a Revit element is selected and used as the focus of the perspective view. A point is created for the camera location, and another point is created for the target location. These are used as inputs to the PerspectiveView.ByEyePointTargetAndElement node, along with the selected element. The output is a new perspective view looking from the eye point toward the target point, with the view extents based on the selected element.


___
## Example File

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)
## Подробности
`PerspectiveView.ByEyePointTargetAndElement` creates a new Revit perspective 3D view using an eye point, a target point, an element, a name, and an `isolateElement` Boolean value.

In the example below, a Revit element is selected and used as the focus of the perspective view. A point is created for the camera location and another for the target location. These are used as inputs to `PerspectiveView.ByEyePointTargetAndElement` along with the selected element. The output is a new perspective view looking from the eye point toward the target point, with the view extents based on the selected element.
___
## Файл примера

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)

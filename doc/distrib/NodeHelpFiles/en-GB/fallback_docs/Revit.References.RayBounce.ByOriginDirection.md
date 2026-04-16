## In Depth
This node performs a ray bounce analysis within the Revit model. Starting from a given origin point and traveling along a specified direction vector, it traces the path of the ray as it intersects elements in the model. When the ray hits a surface, it can continue bouncing off that surface, depending on the number of bounces allowed, simulating light, visibility, or path reflection behavior.

In this example, an element is selected and its' location is used for input origin to node RayBounce.ByOriginDirection, along with a direction, maxBounces and, a view.
___
## Example File

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)

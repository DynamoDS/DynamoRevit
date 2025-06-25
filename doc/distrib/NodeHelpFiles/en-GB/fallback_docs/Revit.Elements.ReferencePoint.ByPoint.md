## In Depth
`ReferencePoint.ByPoint` creates a reference point element in the active Revit family document at the given point location. Note: the family document must be an adaptive component or a mass family. This node differs from "ReferencePoint.ByCoordinates" as it uses a Dynamo point for the location. This is useful because the end user can use direct manipulation to edit the Dynamo point, resulting in the reference point updating as well. For more on direct manipulation, visit this [link](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

In the example below a new reference point is created at coordinates 0,0,1.
___
## Example File

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

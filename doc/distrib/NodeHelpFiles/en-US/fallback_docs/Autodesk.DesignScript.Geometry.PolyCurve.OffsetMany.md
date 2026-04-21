## In Depth
This node offsets a PolyCurve into one or more new PolyCurves based on the offset distance and plane provided.

In this example, a PolyCurve.ByPoints node is used to create the input geometry for the PolyCurve.OffsetMany node. A signed distance value (+ or -) controls the offset direction, an extendCircular Boolean determines how circular edges are handled, and a planeNormal (Vector) defines the normal direction of the plane in which the offset is created.
___
## Example File

![PolyCurve.OffsetMany](./Autodesk.DesignScript.Geometry.PolyCurve.OffsetMany_img.jpg)
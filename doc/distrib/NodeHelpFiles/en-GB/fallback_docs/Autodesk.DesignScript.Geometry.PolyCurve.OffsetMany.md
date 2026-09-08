## In Depth
`PolyCurve.OffsetMany` offsets a PolyCurve into one or more new PolyCurves based on the input offset distance and plane.

In the example below, a `PolyCurve.ByPoints` node creates the input geometry for `PolyCurve.OffsetMany`. A signed distance value (+ or -) controls the offset direction, an `extendCircular` Boolean value determines how circular edges are handled, and a `planeNormal` vector defines the normal direction of the plane in which the offset is created.
___
## Example File

![PolyCurve.OffsetMany](./Autodesk.DesignScript.Geometry.PolyCurve.OffsetMany_img.jpg)

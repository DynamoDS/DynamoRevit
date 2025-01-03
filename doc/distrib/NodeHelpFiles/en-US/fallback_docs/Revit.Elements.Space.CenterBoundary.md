## In Depth
`Space.CenterBoundary` returns a nested list representing the given space's centerline boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the space. The center boundaries are located on the centerline of the wall across all layers within the Revit space. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

If an unbounded or unplaced space is given, a null value is returned.

In the example below, all spaces are collected from the current document and selected view. The center boundaries are then returned.
___
## Example File

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)
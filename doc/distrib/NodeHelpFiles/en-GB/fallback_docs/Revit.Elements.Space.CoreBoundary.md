## In Depth
`Space.CoreBoundary` returns a nested list representing the given space's core boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the space. The Core boundaries are located at the interior or exterior layer of the core that is closest to the room. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

If an unbounded or unplaced space is given, a null value is returned.

In the example below, all spaces are collected from the current document and selected view. The core boundaries are then returned.

___
## Example File

![Space.CoreBoundary](./Revit.Elements.Space.CoreBoundary_img.jpg)

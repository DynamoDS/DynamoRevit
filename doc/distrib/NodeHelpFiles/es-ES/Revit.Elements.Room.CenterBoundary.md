## En detalle:
`Room.CenterBoundary` returns a nested list representing the given room's centerline boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the room. The center boundaries are located on the centerline of the wall across all layers within the Revit room. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The center boundaries are then returned.
___
## Archivo de ejemplo

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)

## In Depth
`Room.CenterBoundary` returns a nested list representing the given room's centerline boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the room. The center boundaries are located on the centerline of the wall across all layers within the Revit room. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-F26DB6DA-A0EC-424D-B656-3BDF47607F4F).

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The center boundaries are then returned.
___
## Example File

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)
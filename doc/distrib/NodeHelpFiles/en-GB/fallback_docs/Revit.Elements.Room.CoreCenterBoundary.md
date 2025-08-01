## In Depth
`Room.CoreCenterBoundary` returns a nested list representing the given room's core center boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the room. Core center boundaries are located at the center of the walls that are defined as core. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The core center boundaries are then returned.
___
## Example File

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)

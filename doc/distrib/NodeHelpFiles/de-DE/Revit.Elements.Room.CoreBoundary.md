## Im Detail
`Room.CoreBoundary` returns a nested list representing the given room's core boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the room. The Core boundaries are located at the interior or exterior layer of the core that is closest to the room. For more information on wall location lines, see this help [article](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The core boundaries are then returned.
___
## Beispieldatei

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)

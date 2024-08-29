## In Depth
`Room.FinishBoundary` returns a nested list representing the given room's finish boundary. In the returned list, the first sublist represents the outermost curves, while subsequent lists represent loops within the room. Finish boundaries occur on Revit rooms on the wall's section that is defined as finish.

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The finish boundaries are then returned.
___
## Example File

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)
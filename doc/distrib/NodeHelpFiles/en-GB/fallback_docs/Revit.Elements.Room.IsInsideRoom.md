## In Depth
`Room.IsInsideRoom` returns a boolean value indicating whether or not the given point is found in the given room.

In the example below, all furniture in the current Revit document is collected, along with all of the rooms. The furniture's location points are then passed into `Room.IsInsideRoom` to check which room the given points occur in (if available). Finally, the furniture is filtered for elements with room locations, and these values are combined into lists.
___
## Example File

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

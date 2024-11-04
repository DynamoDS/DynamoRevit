## Подробности
`Room.Location` returns a Dynamo point that represents a given room's location.

If an unbounded or unplaced room is given, a null value is returned.

In the example below, all rooms are collected from the current document and selected view. The rooms' locations are then returned. Additionally, the room names are previewed in the Dynamo viewport with the `Label.ByPointAndString` node.

___
## Файл примера

![Room.Location](./Revit.Elements.Room.Location_img.jpg)

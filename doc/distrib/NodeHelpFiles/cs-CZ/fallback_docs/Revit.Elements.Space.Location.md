## Podrobnosti
`Space.Location` returns a Dynamo point that represents a given space's location.

If an unbounded or unplaced space is given, a null value is returned.

In the example below, all spaces are collected from the current document and selected view. The spaces' locations are then returned. Additionally, the space names are previewed in the Dynamo viewport with the `Label.ByPointAndString` node.

___
## Vzorový soubor

![Space.Location](./Revit.Elements.Space.Location_img.jpg)

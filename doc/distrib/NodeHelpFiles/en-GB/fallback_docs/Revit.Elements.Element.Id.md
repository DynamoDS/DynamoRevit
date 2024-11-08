## In Depth
`Element.Id` obtains the id of an element in integer form (int64). Element ids are only considered unique in non-workshared documents, while unique ids are stable even in workshared documents.

In the example below, the elementid is obtained for each input wall element. The values are then combined in nested lists with the appropriate wall element. This is useful for export to excel or other external data sources.
___
## Example File

![Element.Id](./Revit.Elements.Element.Id_img.jpg)
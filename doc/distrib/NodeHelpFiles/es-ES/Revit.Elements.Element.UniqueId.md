## In Depth
`Element.UniqueId` obtains the unique id of an element in string form. Unlike an element id, a unique id in Revit is a stable string representation of an element in both workshared and non-workshared models.

In the example below, the unique id is obtained for each input wall element. The values are then combined in nested lists with the appropriate wall element. This is useful for export to excel or other external data sources.
___
## Example File

![Element.UniqueId](./Revit.Elements.Element.UniqueId_img.jpg)
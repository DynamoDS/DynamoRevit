## In Depth
`Element.GetMaterials` returns all materials _(and their ids)_ that exist in a Revit element. Elements with multiple materials will return a list for each element. The input, "paintMaterials" is a boolean toggle to instruct the node to also collect materials that are painted on by the user.

In the example below, the materials for all wall instances in the current document (file) are returned.
___
## Example File

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)
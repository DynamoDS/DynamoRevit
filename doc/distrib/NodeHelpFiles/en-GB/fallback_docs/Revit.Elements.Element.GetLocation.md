## In Depth
`Element.GetLocation` returns a geometrical object in Dynamo that represents the location of each input Revit element. The location of an element is generally determined by its insertion point. For example, the point location of a cylindrical foundation pile is at the top center, while the location of a wall is typically a curve or line.

In the example below, the point locations are returned for all furniture instances _(included shared families)_ in the current document (file).
___
## Example File

![Element.GetLocation](./Revit.Elements.Element.GetLocation_img.jpg)
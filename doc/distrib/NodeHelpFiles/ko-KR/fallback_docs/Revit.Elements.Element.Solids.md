## 상세
`Element.Solids` returns the solid geometry for the given element. The solids are returned as nested lists, as any given element can have more than one solid within it. If a single solid that represents the element is desired, `Solid.ByUnion` may be used on the lists.

In the example below, all walls are collected from the selected view. Walls that were created as in-place families are then removed, and the remaining walls' solids are returned.

___
## 예제 파일

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

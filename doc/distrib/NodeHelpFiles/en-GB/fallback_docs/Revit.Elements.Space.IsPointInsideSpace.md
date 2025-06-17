## In Depth
`Space.IsPointInsideSpace` checks if a given point is inside of a given space. This can be useful when assigning mark values to elements within Revit.

In the example below, all air terminals in the given view in the current Revit document are collected. Their point locations are then compared to the spaces in the given view with `Space.IsPointInsideSpace`. Using list management, sublists are developed for filtering out air terminals that occur within spaces. For more information on using List@Level, please visit this [article](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Example File

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

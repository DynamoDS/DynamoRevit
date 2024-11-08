## In Depth
Given an element that supports hosting of elements _(e.g. Walls)_, `Element.GetHostedElements` returns the elements that rely on the given element. By default, family instances that are hosted to the element are returned. `Element.GetHostedElements` offers the option to include other types of hosted elements. 

These include:
- openings
- elements that are hosted in joined hosts
- embedded walls _(i.e. curtain walls)_
- shared embedded inserts

In the example below, all wall elements are collected from L2. The wall elements are then checked for hosted elements with `Element.GetHostedElements`. This list is then used to create two lists. Walls with doors and walls without doors.
___
## Example File

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
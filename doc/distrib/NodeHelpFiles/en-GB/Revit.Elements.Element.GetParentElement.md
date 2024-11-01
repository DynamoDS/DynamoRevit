## In Depth
When collecting elements in Revit with a category collector, it is possible to collect nested families that are shared. `Element.GetParentElement` helps you identify if a given family instance is nested by identifying its parent element. 

In the example below, all "Chair-Breuer" family instances are grouped by their parent family instance.
___
## Example File

![Element.GetParentElement](./Revit.Elements.Element.GetParentElement_img.jpg)
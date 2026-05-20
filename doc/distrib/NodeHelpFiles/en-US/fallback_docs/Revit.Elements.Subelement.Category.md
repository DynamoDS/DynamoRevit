## In Depth
This node retrieves the category assigned to a Revit subelement. subelements are components or pieces of a larger Revit element, such as layers, mullions, curtain panels, rebars, or nested geometry that can exist within a parent element.

In this example, all elements in the current view are selected and then filtered to show only elements that contain subelements. The subelements are then used as the input to the Subelement.Category node. The output is a list of categories associated with each subelement.
___
## Example File

![Subelement.Category](./Revit.Elements.Subelement.Category_img.jpg)
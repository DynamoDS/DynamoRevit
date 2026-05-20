## In Depth
This node retrieves the internal Revit parameter IDs of subelements.  Those values are the internal Revit parameter IDs, not the parameter display names.

In this example, all elements in the current view are selected and then filtered to show only elements that contain subelements. The subelements are then used as the input to the Subelement.GetAllParameters node. The output is a list of parameter IDs associated with each subelement.  The last node shows the output being used to get the parameter values.

___
## Example File

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)
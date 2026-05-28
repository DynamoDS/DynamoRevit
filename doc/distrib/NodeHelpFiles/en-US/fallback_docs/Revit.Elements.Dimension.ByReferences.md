## In Depth
The node uses the references and places a dimension along the input line in the selected view. The output is a Revit Dimension element.

In this example, several points are created and connected with Line nodes to generate model curves in Revit. The model curves are then converted into Revit curve references.  The references are combined into a list and used as the references input for the Dimension.ByReferences node. Additional lines are created to define the placement and direction of the dimensions. The active Revit view is also supplied as the view input.
___
## Example File

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)
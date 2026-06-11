## En detalle:
`Dimension.ByReferences` places a dimension along the input line in the selected view, using the given references. The output is a Revit Dimension element.

In the example below, several points are created and connected with Line nodes to generate model curves in Revit. The model curves are then converted into Revit curve references. The references are combined into a list and used as the `references` input for `Dimension.ByReferences`. Additional lines are created to define the placement and direction of the dimensions. The active Revit view is also supplied as the `view` input.
___
## Archivo de ejemplo

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

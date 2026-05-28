## In Depth
This node retrieves the spacing rule layout from a divided path element. The divided path is used as the input, and the output is the current spacing layout rule that controls how points are distributed along the path.

In this example, a divided surface is selected from the Revit model and used as the input to modify its parameters. A string node provides the parameter name "Layout", while the Spacing Rule Layout node supplies the spacing rule type value. The graph updates the divided surface layout rule to FixedNumber. The output is the modified divided surface element.


___
## Example File

![Spacing Rule Layout](./DSRevitNodesUI.SpacingRuleLayouts_img.jpg)
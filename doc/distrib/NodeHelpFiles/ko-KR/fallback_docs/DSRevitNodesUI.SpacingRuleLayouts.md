## 상세
`Spacing Rule Layout` retrieves the spacing rule layout from a divided path element. The divided path is used as the input, and the output is the current spacing layout rule that controls how points are distributed along the path.

In the example below, a divided surface is selected from the Revit model and used as the input to modify its parameters. A string node provides the parameter name "Layout", while `Spacing Rule Layout` supplies the spacing rule type value. The graph updates the divided surface layout rule to `FixedNumber`. The output is the modified divided surface element.
___
## 예제 파일

![Spacing Rule Layout](./DSRevitNodesUI.SpacingRuleLayouts_img.jpg)

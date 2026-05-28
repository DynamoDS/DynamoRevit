## In Depth
This node is primarily used in parameter creation and parameter management workflows where a Revit parameter group must be specified programmatically. The input is a string representing a Revit parameter group type id. The output is a GroupType object that can be used downstream in nodes that require a parameter grouping classification.

In this example, several string values representing Revit parameter group type ids are created and combined into a list. The list is then used as the input for the GroupType.ByTypeId node. The output is a list of Revit GroupType objects representing the corresponding parameter groups.
___
## Example File

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
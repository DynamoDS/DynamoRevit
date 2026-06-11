## Podrobnosti
`GroupType.ByTypeId` is primarily used in parameter creation and management workflows where a Revit parameter group must be specified programmatically. The input is a string representing a Revit parameter group type ID. The output is a `GroupType` object that can be used downstream in nodes that require a parameter grouping classification.

In the example below, several strings representing Revit parameter group type IDs are created and combined into a list. The list is then used as the input for `GroupType.ByTypeId`. The output is a list of Revit `GroupType` objects representing the corresponding parameter groups.
___
## Vzorový soubor

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)

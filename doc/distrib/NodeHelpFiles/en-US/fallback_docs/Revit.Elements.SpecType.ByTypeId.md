## In Depth
This node converts a Revit Spec TypeId string into a Revit.Elements.SpecType object. Spec Types define the kind of data a parameter represents in Revit, such as length, area, angle, material, force, text, integer, and many others. This is commonly used when creating shared parameters, project parameters, or parameter definitions in newer versions of Revit where Autodesk replaced the older ParameterType system with Forge TypeIds.

In this example, a string representing a Revit Spec TypeId is provided to the SpecType.ByTypeId node. The node converts the string into a Revit.Elements.SpecType object. This SpecType is then used downstream when creating a new parameter definition so Revit understands the type of data the parameter should store. The output of this node is a Revit SpecType object.


___
## Example File

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
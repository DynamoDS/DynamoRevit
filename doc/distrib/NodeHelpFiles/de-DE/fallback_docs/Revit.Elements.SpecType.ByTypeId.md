## Im Detail
`SpecType.ByTypeId` converts a Revit Spec TypeId string into a `SpecType` object. Spec Types define the kind of data a parameter represents in Revit, such as length, area, angle, material, force, text, integer, and many others. This is commonly used when creating shared parameters, project parameters, or parameter definitions in newer versions of Revit where Autodesk replaced the older `ParameterType` system with Forge TypeIds.

In the example below, a string representing a Revit Spec TypeId is provided to `SpecType.ByTypeId`. The output is a `SpecType` object that can be used downstream when creating a new parameter definition so Revit understands the type of data the parameter should store.
___
## Beispieldatei

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

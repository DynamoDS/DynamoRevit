## Description approfondie
`Parameter.GroupType` returns the given parameter's GroupType.

In Revit, parameter groups for non-shared parameters are predefined, whereas shared parameters allow you to define custom groups. For more information on working with shared parameters and groups, visit this help article: [Create Shared Parameter Files, Groups, and Parameters](https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-94EA2B8E-2C00-4D29-8D5A-C7C6664DE9CE)

If no GroupType is found, a null value is returned.

In the example below, all parameters are returned for the current document's project information. The GroupTypes are returned as well.
___
## Exemple de fichier

![Parameter.GroupType](./Revit.Elements.Parameter.GroupType_img.jpg)

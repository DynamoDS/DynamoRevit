## Description approfondie
`FloorType.ByName` returns a floor type for the given name in the current Revit document. If the floor type does not exist, a null value is returned.

In the example below, all floors present in the current Revit document are collected. The floors' types are then collected by getting the name of the floor and using `FloorType.ByName` to retrieve the floor type.
___
## Exemple de fichier

![FloorType.ByName](./Revit.Elements.FloorType.ByName_img.jpg)

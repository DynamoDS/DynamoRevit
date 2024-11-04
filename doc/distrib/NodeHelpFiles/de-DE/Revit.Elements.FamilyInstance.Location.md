## Im Detail
`FamilyInstance.Location` returns a Dynamo point for the given family instance. If there is no location, a null value is returned. `FamilyInstance.Location` works on point-based element, and will not return a location for curve based element in Revit, _e.g. a wall or beam element_.

In the example below, all door family instances in the current view of the current document are collected. The doors' locations are then returned with `FamilyInstance.Location`.

In the case of this example, curtain wall doors are returning null. Curtain panels locations are available via curtain panel nodes.
___
## Beispieldatei

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

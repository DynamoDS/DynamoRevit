## In Depth
`Mullion.AsFamilyInstance` converts the input mullion element to a Revit family instance. This is useful for obtaining element parameters, or extracting type properties from the mullion.

In the example below, all curtain panels are obtained from a curtain wall element selected by element id. The mullions are then obtained from the curtain wall panels. Finally, the mullions are then converted to family instances.
___
## Example File

![Mullion.AsFamilyInstance](./Revit.Elements.Mullion.AsFamilyInstance_img.jpg)
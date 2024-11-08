## En detalle:
Similar to `Revit.Elements.FamilyType.ByFamilyAndName`, `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` returns the family type definition from the current document (if available). This is similar to `Revit.Elements.FamilyType.ByFamilyAndName`. However, instead of using a family definition, this node relies on string input for both values. If the family type is not available in the current document, a null value is returned.

In the example below, a door family type, "36" x 84", from family "Door-Passage-Single-Flush" is returned.
___
## Archivo de ejemplo

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)

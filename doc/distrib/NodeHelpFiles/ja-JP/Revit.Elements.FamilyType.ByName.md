## 詳細
`FamilyType.ByName` will attempt to retrieve the given family type of the given name from the current document. If the family type is not available in the current document, a null value is returned.

Note: `FamilyType.ByName` searches family type definitions in order of the parent family's creation (by element id). If multiple parent families have a type definition with the same name, the first one found is returned. For a more concise lookup of family types use, `FamilyType.ByFamilyAndName` or `FamilyType.ByFamilyNameAndTypeName`

In the example below, a door family type, "36" x 84", from family "Door-Passage-Single-Flush" is returned.
___
## サンプル ファイル

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)

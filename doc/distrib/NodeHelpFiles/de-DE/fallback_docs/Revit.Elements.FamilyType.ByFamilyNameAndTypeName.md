## Im Detail
Ähnlich wie `Revit.Elements.FamilyType.ByFamilyAndName` gibt `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` die Familientypdefinition aus dem aktuellen Dokument zurück (falls verfügbar). Dies ähnelt `Revit.Elements.FamilyType.ByFamilyAndName`. Dieser Block verwendet jedoch keine Familiendefinition, sondern für beide Werte Zeichenfolgeneingaben. Wenn der Familientyp im aktuellen Dokument nicht verfügbar ist, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel wird der Türfamilientyp "36" x 84" aus der Familie "Tür-Durchgang-Einflügelig" zurückgegeben.
___
## Beispieldatei

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)

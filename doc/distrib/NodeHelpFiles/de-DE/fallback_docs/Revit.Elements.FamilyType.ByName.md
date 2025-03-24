## Im Detail
`FamilyType.ByName` versucht, den angegebenen Familientyp des angegebenen Namens aus dem aktuellen Dokument abzurufen. Wenn der Familientyp im aktuellen Dokument nicht verfügbar ist, wird ein Nullwert zurückgegeben.

Anmerkung: `FamilyType.ByName` durchsucht Familientypdefinitionen in der Erstellungsreihenfolge der übergeordneten Familie (nach Element-ID). Haben mehrere übergeordnete Familien eine Typdefinition mit demselben Namen, wird die zuerst gefundene Definition zurückgegeben. Verwenden Sie zur einfacheren Suche nach Familientypen `FamilyType.ByFamilyAndName` oder `FamilyType.ByFamilyNameAndTypeName`.

Im folgenden Beispiel wird der Türfamilientyp "36" x 84" aus der Familie "Tür-Durchgang-Einflügelig" zurückgegeben.
___
## Beispieldatei

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)

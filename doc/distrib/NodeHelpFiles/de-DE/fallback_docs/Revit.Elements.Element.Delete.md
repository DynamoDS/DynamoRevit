## Im Detail
`Element.Delete` funktioniert auf die gleiche Weise wie die Option Löschen in der Revit-Benutzeroberfläche. Dabei werden das Eingabeelement und alle von diesem abhängigen Elemente gelöscht.

Wenn Sie z. B. eine Wand mit Türen löschen, werden auch die Türen gelöscht. Die Ausgabe besteht aus einer verschachtelten Liste mit den Element-IDs der Elemente, die gelöscht wurden. Anmerkung: Dieser Block sollte am besten im manuellen Ausführungsmodus in Dynamo verwendet werden.

Im folgenden Beispiel werden alle Familienexemplare des Typs "Hilfe-Schaltfläche" aus dem aktuellen Dokument (Datei) gelöscht.
___
## Beispieldatei

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

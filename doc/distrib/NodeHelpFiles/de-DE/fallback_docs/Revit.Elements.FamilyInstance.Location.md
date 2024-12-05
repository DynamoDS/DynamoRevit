## Im Detail
`FamilyInstance.Location` gibt einen Dynamo-Punkt für das angegebene Familienexemplar zurück. Wenn keine Position vorhanden ist, wird ein Nullwert zurückgegeben. `FamilyInstance.Location` funktioniert mit punktbasierten Elementen und gibt keine Position für kurvenbasierte Elemente in Revit zurück, _z. B. für Wände oder Trägerelemente_.

Im folgenden Beispiel werden alle Türfamilienexemplare in der aktuellen Ansicht des aktuellen Dokuments gesammelt. Die Positionen der Türen werden dann mit `FamilyInstance.Location` zurückgegeben.

In diesem Beispiel geben Fassadentüren einen Nullwert zurück. Die Positionen von Fassadenelementen sind über Fassadenelementblöcke verfügbar.
___
## Beispieldatei

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

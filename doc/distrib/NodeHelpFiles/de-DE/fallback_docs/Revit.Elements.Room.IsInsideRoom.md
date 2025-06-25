## Im Detail
`Room.IsInsideRoom` gibt einen booleschen Wert zurück, der angibt, ob der angegebene Punkt im angegebenen Raum gefunden wurde.

Im folgenden Beispiel werden alle Möbel im aktuellen Revit-Dokument zusammen mit allen Räumen gesammelt. Die Positionspunkte der Möbel werden dann an `Room.IsInsideRoom` übergeben, um zu prüfen, in welchem Raum sich die angegebenen Punkte befinden (falls verfügbar). Schließlich werden die Möbel nach Elementen mit Raumpositionen gefiltert, und diese Werte werden in Listen kombiniert.
___
## Beispieldatei

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

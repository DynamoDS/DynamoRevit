## Im Detail
Mit `Rooms By Status` werden alle Räume im Dokument gruppiert nach Status zurückgegeben.

Der Status enthält Räume, die:
- korrekt platziert und begrenzt sind
- nicht platziert sind (zuvor aus einer Ansicht gelöscht, jedoch nicht aus dem Modell)
- nicht umschlossen sind (unbegrenzte Räume ohne berechnete Fläche)
- redundant sind (Räume, die einen umschlossenen Raum mit anderen Räumen gemeinsam haben)

Im folgenden Beispiel werden die platzierten Räume nach ihrer Fläche abgefragt.
___
## Beispieldatei

![Rooms By Status](./DSRevitNodesUI.RoomsByStatus_img.jpg)

## Im Detail
Dieser Block beschriftet Revit-Elemente anhand einer Ansicht, eines Elements, einer Position, der horizontal-Angabe (wenn nein, wird die Beschriftung basierend auf dem Element ausgerichtet) und von addLeader als Eingaben.

In diesem Beispiel wird eine Tür in der Ansicht “Studio Live Work Core B” ausgewählt. Die Position dieser Tür wird extrahiert und dann zusammen mit den booleschen Werten für die horizontal-Angabe und addLeader als ursprüngliche Eingabe für Tag.ByElementAndLocation verwendet. Die ursprüngliche Position wird so geändert, dass die Beschriftungsposition nicht direkt über dem Element liegt, indem der Tag.SetHeadLocation-Block verwendet wird.

___
## Beispieldatei

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

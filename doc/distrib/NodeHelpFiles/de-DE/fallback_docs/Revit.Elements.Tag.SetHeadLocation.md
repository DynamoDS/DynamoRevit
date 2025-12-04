## Im Detail
Dieser Block nimmt eine Beschriftung entgegen und ändert ihre Textposition. Dadurch können wir ein konsistentes Platzierungsverhalten automatisieren, sodass Beschriftungen direkt über dem Element platziert werden, das sie beschriften.

In diesem Beispiel wird eine Tür in der Ansicht “Studio Live Work Core B” ausgewählt. Die Position dieser Tür wird extrahiert und dann zusammen mit den booleschen Werten für die horizontal-Angabe und addLeader als ursprüngliche Eingabe für Tag.ByElementAndLocation verwendet. Die ursprüngliche Position wird so geändert, dass die Beschriftungsposition nicht direkt über dem Element liegt, indem der Tag.SetHeadLocation-Block verwendet wird.

___
## Beispieldatei

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

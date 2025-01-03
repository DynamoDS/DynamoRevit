## Im Detail
`Room.CoreBoundary` gibt eine verschachtelte Liste zurück, die die angegebene Kernbegrenzung des Raums darstellt. In der zurückgegebenen Liste stellt die erste Unterliste die äußersten Kurven dar, während nachfolgende Listen Schleifen innerhalb des Raums darstellen. Die Kernbegrenzungen befinden sich auf dem inneren oder äußeren Layer des Kerns, der am nächsten zum Raum liegt. Weitere Informationen zu Wandpositionslinien finden Sie in diesem [Hilfeartikel](https://help.autodesk.com/view/RVT/2024/DEU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Wenn ein nicht begrenzter oder nicht platzierter Raum angegeben wird, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel werden alle Räume aus dem aktuellen Dokument und der ausgewählten Ansicht gesammelt. Dann werden die Kernbegrenzungen zurückgegeben.
___
## Beispieldatei

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)

## Im Detail
`Space.CenterBoundary` gibt eine verschachtelte Liste zurück, die die Achsenbegrenzung des angegebenen Raums darstellt. In der zurückgegebenen Liste stellt die erste Unterliste die äußersten Kurven dar, während die nachfolgenden Listen Konturen im Raum darstellen. Die Achsenbegrenzungen befinden sich auf der Mittellinie der Wand über alle Layer innerhalb des Revit-Raums hinweg. Weitere Informationen zu Wandpositionslinien finden Sie in diesem [Hilfeartikel](https://help.autodesk.com/view/RVT/2024/DEU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Wenn ein nicht begrenzter oder nicht platzierter Raum angegeben ist, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel werden alle Räume aus dem aktuellen Dokument und der ausgewählten Ansicht gesammelt. Die Achsenbegrenzungen werden dann zurückgegeben.
___
## Beispieldatei

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)

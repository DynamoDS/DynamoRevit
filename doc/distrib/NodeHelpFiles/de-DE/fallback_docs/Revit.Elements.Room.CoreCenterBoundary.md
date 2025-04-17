## Im Detail
`Room.CoreCenterBoundary` gibt eine verschachtelte Liste zurück, die die Begrenzung der Wandkernachse des angegebenen Raums darstellt. In der zurückgegebenen Liste stellt die erste Unterliste die äußersten Kurven dar, während die nachfolgenden Listen Schleifen im Raum darstellen. Die Begrenzungen der Wandkernachse befinden sich in der Mitte der Wände, die als Kern definiert sind. Weitere Informationen zu Wandpositionslinien finden Sie in diesem [Hilfeartikel](https://help.autodesk.com/view/RVT/2024/DEU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Wenn ein nicht begrenzter oder nicht platzierter Raum angegeben wird, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel werden alle Räume aus dem aktuellen Dokument und der ausgewählten Ansicht gesammelt. Dann werden die Begrenzungen der Wandkernachse dann zurückgegeben.
___
## Beispieldatei

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)

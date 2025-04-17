## Im Detail
`Room.FinishBoundary` gibt eine verschachtelte Liste zurück, die die Begrenzung der nichttragenden Schicht des angegebenen Raums darstellt. In der zurückgegebenen Liste stellt die erste Unterliste die äußersten Kurven dar, während nachfolgende Listen Schleifen innerhalb des Raums darstellen. Die von diesem Block zurückgegebene Raumbegrenzung befindet sich an der Oberfläche innerhalb des Revit-Raums. Weitere Informationen zu Wandpositionslinien finden Sie in diesem [Hilfeartikel](https://help.autodesk.com/view/RVT/2024/DEU/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Wenn ein nicht begrenzter oder nicht platzierter Raum angegeben wird, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel werden alle Räume aus dem aktuellen Dokument und der ausgewählten Ansicht gesammelt. Dann werden die Begrenzungen der nichttragenden Schicht zurückgegeben.
___
## Beispieldatei

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)

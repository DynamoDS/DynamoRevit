## Im Detail
Dieser Block extrahiert die tatsächliche Revit-Elementreferenz einer ausgewählten Referenzebene. Dies ist nützlich, wenn Sie diese Ebene als Basisreferenz für Geometrie oder Bemaßungen in Revit verwenden müssen.

Beispiel:
In diesem Diagramm werden zwei Punkte mithilfe von Koordinaten definiert, und zwischen ihnen wird mit ReferencePlane.ByStartPointEndPoint eine Referenzebene erstellt. Diese Referenzebene wird dann mit ReferencePlane.ElementPlaneReference verknüpft, wodurch die native Revit-Referenz der Ebene ausgegeben wird, sodass sie für Basisbauteil- oder Ausrichtungsaufgaben verwendet werden kann.
___
## Beispieldatei

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

## Im Detail
Der Block Revision.Issued in Dynamo wird verwendet, um zu prüfen, ob eine Änderung in Revit als ausgegeben markiert ist. Er gibt den Wert True oder False (boolescher Wert) zurück, sodass Teams den Status von Änderungen schnell überprüfen können, ohne die Revit-Änderungseinstellungen öffnen zu müssen.

In diesem Diagramm wird der Block Select Revision verwendet, um eine Änderung aus dem Projekt auszuwählen. Der Revision.Issued-Block prüft dann, ob die ausgewählte Änderung ausgegeben wurde, und das Ergebnis wird im Watch-Block als True oder False angezeigt. Dadurch können Sie den Ausgabestatus einer Änderung auf einfache Weise direkt über Dynamo bestätigen.

___
## Beispieldatei

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)

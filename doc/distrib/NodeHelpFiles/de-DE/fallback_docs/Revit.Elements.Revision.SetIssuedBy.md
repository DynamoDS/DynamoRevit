## Im Detail
Der Block Revision.SetIssuedBy in Dynamo wird zum Festlegen oder Aktualisieren des Werts “Ausgegeben von” für eine Änderung in Revit verwendet. Dies hilft bei der Automatisierung der Änderungskontrolle, da erfasst wird, wer die Änderung ausgegeben hat, und so sichergestellt werden kann, dass die Dokumentation ohne manuelle Bearbeitung in Revit verständlich und einheitlich ist.

In diesem Diagramm wird der Block Select Revision verwendet, um die erforderliche Änderung auszuwählen, und eine Zeichenfolgeneingabe (z. B. ABC) gibt den Namen des Ausstellers an. Der Block Revision.SetIssuedBy wendet diesen Wert dann auf die ausgewählte Änderung an, wobei das Feld “Ausgegeben von” direkt im Revit-Modell aktualisiert wird.

___
## Beispieldatei

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)

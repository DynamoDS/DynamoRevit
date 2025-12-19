## Im Detail
Mit dem Block Revision.SetIssued in Dynamo können Sie steuern, ob eine ausgewählte Änderung in Revit als ausgegeben oder nicht ausgegeben markiert wird. Die Eingaben sind ein Änderungselement und eine boolesche Eingabe (True/False), sodass Benutzer die direkte Kontrolle über den Änderungsstatus haben, ohne ihn manuell in Revit bearbeiten zu müssen.
In diesem Diagramm wird der Block Select Revision verwendet, um eine bestimmte Änderung auszuwählen (z. B. "Seq. 1 - Schematic Design"). Der Boolean-Block gibt einen True/False-Wert an, der dann mit dem Block Revision.SetIssued verknüpft wird, um den Ausgabestatus der Änderung automatisch zu aktualisieren.

___
## Beispieldatei

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

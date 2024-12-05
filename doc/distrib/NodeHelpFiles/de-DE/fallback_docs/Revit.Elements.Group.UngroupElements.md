## Im Detail
`Group.UngroupElements` hebt die Gruppierung des angegebenen Gruppenexemplars auf, sodass sich die Elemente direkt im Modell befinden. Beachten Sie, dass diese Aktion destruktiv ist und es nicht einfach ist, die Gruppenmitglieder wieder zu einer Gruppe hinzuzufügen.

Im folgenden Beispiel werden alle Modellgruppen des Typs "Tribüne fortlaufend" aus dem aktiven Revit-Dokument gesammelt. Die Gruppierung der Exemplare wird dann mit `Group.UngroupElements` aufgehoben.

___
## Beispieldatei

![Group.UngroupElements](./Revit.Elements.Group.UngroupElements_img.jpg)

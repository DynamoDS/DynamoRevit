## Im Detail
`Space.IsPointInsideSpace` überprüft, ob sich ein bestimmter Punkt innerhalb eines angegebenen Raums befindet. Dies kann nützlich sein, wenn Sie Elementen in Revit Markierungswerte zuweisen möchten.

Im folgenden Beispiel werden alle Luftdurchlässe in der angegebenen Ansicht im aktuellen Revit-Dokument gesammelt. Die Positionen der Punkte werden dann mit `Space.IsPointInsideSpace` mit den Räumen in der angegebenen Ansicht verglichen. Mithilfe der Listenverwaltung werden Unterlisten zum Herausfiltern von Luftdurchlässen erstellt, die in Räumen vorkommen. Weitere Informationen zur Verwendung von List@Level finden Sie in diesem [Artikel](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Beispieldatei

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

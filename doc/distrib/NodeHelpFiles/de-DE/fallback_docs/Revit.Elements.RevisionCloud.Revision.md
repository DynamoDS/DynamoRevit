## Im Detail
Dieser Block extrahiert das mit einer bestimmten Änderungswolke in Revit verknüpfte Änderungselement. Er stellt die mit dieser Cloud verknüpften Änderungsdaten bereit, sodass Benutzer Änderungsdetails programmgesteuert innerhalb ihres Projekts prüfen, verfolgen oder validieren können.

In diesem Beispiel wird ein Rechteck mit Zahlen-Schiebereglern für Breite und Länge erstellt, dann in Kurven aufgelöst und umgekehrt, um die richtige Ausrichtung zu erzielen. Diese Kurven werden zusammen mit einer ausgewählten Ansicht (L1_SD) und einer ausgewählten Änderung (Seq. 2 - Not For Construction) verwendet, um über den Block RevisionCloud.ByCurve eine Änderungswolke zu generieren. Die resultierende Änderungswolke ist mit dem Block RevisionCloud.Revision verknüpft, der die mit dieser Wolke verknüpfte Änderung abruft und ausgibt. Dadurch wird sichergestellt, dass Benutzer bestätigen können, welche Änderung mit welcher Änderungswolke verknüpft ist.
___
## Beispieldatei

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)

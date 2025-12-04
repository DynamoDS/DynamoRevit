## Im Detail
Dieser Block extrahiert die Kurvenkonturen (in der Regel Bogen und Linien), die den sichtbaren Umfang einer Änderungswolke bilden. Jedes Segment der Wolke wird als Kurvenobjekt (in der Regel ein Bogen) dargestellt, das der “Blasenform” der Änderungsmarkierung in einer Ansicht oder einem Plan entspricht.

In diesem Beispiel wird ein Rechteck erstellt und die Bemaßungen mithilfe von Zahlen-Schiebereglern definiert, dann in Kurven aufgelöst und seine Ausrichtung umgekehrt. Diese Kurven werden zusammen mit einer ausgewählten Ansicht (Lageplan) und einer ausgewählten Änderung (Seq. 2 - Not For Construction) verwendet, um eine Änderungswolke mit dem Block RevisionCloud.ByCurve zu erstellen. Die erstellte Änderungswolke wird dann mit dem RevisionCloud.Curves-Block verknüpft, der die definierenden Kurven dieser Wolke extrahiert und anzeigt. Dies hilft Benutzern bei der Überprüfung der Änderungswolkengeometrie und bietet Flexibilität für die Wiederverwendung oder weitere Automatisierung.
___
## Beispieldatei

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)

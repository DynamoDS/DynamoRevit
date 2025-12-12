## Im Detail
Dieser Block ruft die Transformationsmatrix ab, die auf ein Revit-Verknüpfungselement im Grundmodell angewendet wurde.
Mit anderen Worte: Sie gibt die Positions-, Drehungs- und Skalierungstransformation zurück, die das Koordinatensystem des verknüpften Elements dem Koordinatensystem des Revit-Basisprojekts zuordnet.
Dies ist nützlich, wenn Sie Geometrie zwischen verknüpften Modellen ausrichten, analysieren oder bearbeiten müssen.

In diesem Beispiel werden alle verknüpften Revit-Elemente auf Ebene L3 ausgewählt und in LinkElement.LinkTransform eingegeben. Die Ausgabe ist die Positions-, Drehungs- und Skalierungstransformation des verknüpften Elements.
___
## Beispieldatei

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

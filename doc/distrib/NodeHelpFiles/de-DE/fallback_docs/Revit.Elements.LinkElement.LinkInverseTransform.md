## Im Detail
Dieser Block ruft die umgekehrte Transformationsmatrix eines bestimmten Revit-Verknüpfungselements ab. In Revit können verknüpfte Modelle mit Transformationen (Verschiebung, Drehung, Skalierung) positioniert werden. Dieser Block ermöglicht es Ihnen, die Umkehrung dieser Transformation zu erhalten, indem Sie effektiv Koordinaten aus dem Raum des verknüpften Modells zurück in das Koordinatensystem des Revit-Grundmodells konvertieren.

In diesem Beispiel werden alle verknüpften Revit-Elemente auf Ebene L3 ausgewählt und in LinkElement.LinkInverseTransform eingegeben. Die Ausgabe ist das Koordinatensystem des Revit-Grundmodells.
___
## Beispieldatei

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

## Im Detail

Gibt den Einheitentyp eines Parameters zurück.

Im folgenden Beispiel werden alle Elementparameter extrahiert und als Eingabe verwendet. Ziel ist es, den Einheitentyp für jeden Parameter anzuzeigen.
Wenn Element.Parameters beispielsweise "Base Diameter : 1’ – 3 ¼”" anzeigt, lautet die Ausgabe von Parameter.Unit "Unit (Name = Feet or Meters)".
Wenn ein Parameter keine Einheit umfasst (z. B. Element.Parameters = Apply Cut : No), gibt Parameter.Unit null zurück.
Da Dynamo selbst keine Einheiten aufweist, ist es wichtig, eingehende Einheitentypen zu identifizieren, um genaue Berechnungen durchführen zu können. Dieser Block hilft Dynamo, diese Einheitendaten zu erkennen und zu verarbeiten.

___
## Beispieldatei

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

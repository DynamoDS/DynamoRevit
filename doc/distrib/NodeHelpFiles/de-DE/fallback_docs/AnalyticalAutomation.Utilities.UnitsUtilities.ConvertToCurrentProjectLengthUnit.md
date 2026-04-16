## Im Detail
Dieser Block verwendet einen numerischen Längenwert und eine Einheitentypkennung und konvertiert den Eingabewert in die Längeneinheiten des aktiven Revit-Projekts. Die Ausgabe ist ein double-Wert, der das konvertierte Ergebnis darstellt.

In diesem Beispiel liefert ein Zahlen-Schieberegler einen Längenwert, und eine Einheit (z. B. Meter) wird ausgewählt, um die Unit.TypeId-Zeichenfolge abzurufen. Beide sind mit dem Block UnitsUtilities.ConvertToCurrentProjectLengthUnit verknüpft, der den konvertierten Längenwert basierend auf den Einheiteneinstellungen des Projekts zurückgibt.
___
## Beispieldatei

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

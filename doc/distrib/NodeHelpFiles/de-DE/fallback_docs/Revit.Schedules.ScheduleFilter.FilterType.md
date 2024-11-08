## Im Detail
`ScheduleFilter.FilterType` gibt die Methode für den Eingabefilter zurück.
Folgende Filtertypen sind möglich:

- Gleich - Der Feldwert entspricht dem angegebenen Wert.
- Ungleich - Der Feldwert entspricht nicht dem angegebenen Wert.
- Größer als - Der Feldwert ist größer als der angegebene Wert.
- Größer oder gleich - Der Feldwert ist größer oder gleich dem angegebenen Wert.
- Kleiner als - Der Feldwert ist kleiner als der angegebene Wert.
- Kleiner oder gleich - Der Feldwert ist kleiner als oder gleich dem angegebenen Wert.
- Enthält - Bei einem Zeichenfolgenfeld enthält der Feldwert die angegebene Zeichenfolge.
- Enthält nicht - Bei einem Zeichenfolgenfeld enthält der Feldwert die angegebene Zeichenfolge nicht.
- Beginnt mit - Bei einem Zeichenfolgenfeld beginnt der Feldwert mit der angegebenen Zeichenfolge.
- Beginnt nicht mit - Bei einem Zeichenfolgenfeld beginnt der Feldwert nicht mit der angegebenen Zeichenfolge.
- Endet mit - Bei einem Zeichenfolgenfeld endet der Feldwert mit der angegebenen Zeichenfolge.
- Endet nicht mit - Bei einem Zeichenfolgenfeld endet der Feldwert nicht mit der angegebenen Zeichenfolge.
- Ist globalem Parameter zugeordnet - Das Feld ist mit einem angegebenen globalen Parameter eines kompatiblen Typs verknüpft
- Ist nicht globalem Parameter zugeordnet - Das Feld ist nicht mit einem angegebenen globalen Parameter eines kompatiblen Typs verknüpft

Im folgenden Beispiel wird die erste Bauteilliste aus der aktuellen Revit-Datei gesammelt. Die Bauteillistenansicht wird dann auf Filter überprüft, und der einzige angewendete Filter ist der Filtertyp "Zeichenfolge endet nicht mit".
___
## Beispieldatei

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

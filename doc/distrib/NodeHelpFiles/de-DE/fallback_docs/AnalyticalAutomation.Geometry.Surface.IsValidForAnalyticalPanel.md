## Im Detail
Dieser Block wertet eine angegebene Oberfläche aus, um zu bestimmen, ob sie zur Definition einer analytischen Schale verwendet werden darf. Eine zulässige Oberfläche ist in der Regel planar und durchgehend und eignet sich für die Konvertierung in eine analytische Darstellung in der Berechnungsmodellumgebung von Revit.

In diesem Beispiel werden die Flächen eines Plattenelements aus dem Projekt erfasst, und die obere Fläche wird als Eingabe für den Block bereitgestellt. Der Block gibt ein boolesches Ergebnis zurück, das angibt, ob die ausgewählte Oberfläche die Anforderungen zum Erstellen einer analytischen Schale erfüllt, zusammen mit einer optionalen Meldung, die eventuelle Probleme beschreibt, die während der Validierung aufgetreten sind.
___
## Beispieldatei

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

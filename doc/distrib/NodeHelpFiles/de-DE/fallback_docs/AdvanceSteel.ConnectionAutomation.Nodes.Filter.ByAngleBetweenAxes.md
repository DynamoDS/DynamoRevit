## Im Detail
Dieser Block gibt eine Liste von Verbindungsblöcken zurück, gefiltert nach einem minimalen und maximalen Gradbereich und der ausgewählten Achse.

In diesem Beispiel wurden eine W-förmige Stütze und eine abgewinkelte HSS-Strebe ausgewählt, und die Ausgabe besteht aus einer Liste von Wörterbüchern mit den Eigenschaften "Akzeptiert" und "Abgelehnt". Der akzeptierte Wert ist ein Verbindungsblock, der beim Vergleich der X-Achse der beiden Elemente im Bereich von 0 bis 60 Grad liegt. Wenn der Maximalwert 45 Grad betrüge, würde der Verbindungsblock abgelehnt werden. Die Parameter `indexFirst` und `indexSecond` werden auf 'use levels' gesetzt, um sie an der Eingabedatenstruktur auszurichten.
___
## Beispieldatei

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

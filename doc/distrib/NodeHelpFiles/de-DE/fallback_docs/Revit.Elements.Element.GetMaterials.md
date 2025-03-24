## Im Detail
`Element.GetMaterials` gibt alle in einem Revit-Element vorhandenen Materialien _(und deren IDs)_ zurück. Bei Elementen mit mehreren Materialien wird eine Liste für jedes Element zurückgegeben. Die Eingabe "paintMaterials" ist ein boolescher Schalter, mit dem der Block angewiesen wird, auch die vom Benutzer mit Farbe versehenen Materialien zu sammeln.

Im folgenden Beispiel werden die Materialien für alle Wandexemplare im aktuellen Dokument (Datei) zurückgegeben.
___
## Beispieldatei

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)

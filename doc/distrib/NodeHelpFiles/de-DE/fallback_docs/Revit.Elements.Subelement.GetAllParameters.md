## Im Detail
`Subelement.GetAllParameters` ruft die internen Revit-Parameter-IDs von Teilelementen ab, nicht die Anzeigenamen von Parametern.

Im folgenden Beispiel werden alle Elemente in der aktuellen Ansicht ausgewählt und gefiltert, sodass nur die Elemente angezeigt werden, die Teilelemente enthalten. Die Teilelemente werden dann als Eingabe für `Subelement.GetAllParameters` verwendet. Die Ausgabe ist eine Liste von Parameter-IDs, die mit den einzelnen Teilelementen verknüpft sind. Der letzte Block zeigt die Ausgabe, die zum Abrufen der Parameterwerte verwendet wird.

___
## Beispieldatei

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)

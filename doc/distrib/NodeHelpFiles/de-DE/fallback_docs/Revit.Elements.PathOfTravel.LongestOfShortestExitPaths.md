## Im Detail
`PathOfTravel.LongestOfShortestExitPaths` erstellt einen Bewegungspfad, der die längste/kürzeste Route von einer Gruppe von Punkten zu einem oder mehreren Ausgangspunkten darstellt. Jeder Startpunkt wird mit den verfügbaren Ausgängen verglichen, und Revit ermittelt für jeden Ausgang den nächstgelegenen gültigen Ausgangspfad. Der Block gibt dann den Pfad mit der größten Streckenlänge unter diesen Ergebnissen für den kürzesten Pfad zurück.

Im folgenden Beispiel werden zunächst eine Ebenen- und eine Grundrissansicht erstellt, um die erforderliche Revit-Ansicht für die Berechnung des Bewegungspfads bereitzustellen. Mehrere Punkte werden mit `Point.ByCoordinates` generiert und in einer Liste mit möglichen Ausgangspositionen zusammengefasst. `PathOfTravel.LongestOfShortestExitPaths` verwendet die Grundrissansicht und die Liste der Zielpunkte zum Berechnen der Bewegungspfade und gibt den Pfad mit der größten Streckenlänge zurück.
___
## Beispieldatei

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

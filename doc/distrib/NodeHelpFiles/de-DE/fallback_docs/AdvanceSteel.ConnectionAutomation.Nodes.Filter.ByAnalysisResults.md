## Im Detail
Dieser Block filtert eine Liste von ConnectionNodes, indem geprüft wird, ob der Kraftwert bei einem angegebenen Index in einen definierten Bereich fällt. Die Kraftdaten stammen entweder aus den Ergebnissen der Tragwerksbemessung oder aus dem Revit-Berechnungsmodell und werden nach dem ausgewählten Ergebnistyp gefiltert (z. B. Fx, Fy, Fz, Mx, My, Mz).

In diesem Beispiel wird ein Satz von Stützenelementen ausgewählt und basierend auf der Fz-Kraftkomponente unter Verwendung des ausgewählten Analyseergebnisses und Lastfalls ausgewertet. Nur die Elemente, deren Fz-Wert innerhalb des angegebenen Kraftbereichs liegt, werden als zulässige Verbindungen zurückgegeben.
___
## Beispieldatei

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

## Podrobnosti
Uzel `PathOfTravel.LongestOfShortestExitPaths` vytvoří trasu cesty představující nejdelší a nejkratší trasu ze skupiny bodů do jednoho nebo více výstupních bodů. Každý počáteční bod je testován s dostupnými východy a aplikace Revit pro každý z nich určí nejbližší platnou trajektorii východu. Uzel pak vrátí trajektorii s největší vzdáleností pohybu mezi výsledky nejkratší cesty.

V níže uvedeném příkladu se nejprve vytvoří půdorysný pohled úrovně a podlaží, který poskytuje požadovaný pohled aplikace Revit pro výpočet trasy cesty. Několik bodů je generováno pomocí uzlu `Point.ByCoordinates` a sloučeno do seznamu představujícího možná umístění východů. Uzel `PathOfTravel.LongestOfShortestExitPaths` použije půdorysný pohled podlaží a seznam cílových bodů k výpočtu trajektorií a vrátí trajektorii s největší vzdáleností pohybu.
___
## Vzorový soubor

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

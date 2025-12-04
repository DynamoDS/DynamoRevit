## In profondità
Questo nodo crea una vista di prospetto da un ElevationMarker esistente specificando l'indice del contrassegno. Ogni ElevationMarker in Revit può ospitare fino a quattro viste di prospetto singole, una per ciascuna direzione (North, South, East e West). Questo nodo consente di generare uno di questi prospetti direzionali facendo riferimento al contrassegno e al numero di indice desiderato.

In questo esempio, viene creato un contrassegno di prospetto che è poi utilizzato come input elevationMarker nel nodo ElevationMarker.CreateElevationByMarkerIndex insieme a planView e index (0,1,2,3).

___
## File di esempio

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

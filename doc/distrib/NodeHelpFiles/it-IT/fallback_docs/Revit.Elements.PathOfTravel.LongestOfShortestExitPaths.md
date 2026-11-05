## In profondità
`PathOfTravel.LongestOfShortestExitPaths` crea una traiettoria di movimento che rappresenta il percorso più lungo tra quelli più brevi da un gruppo di punti ad uno o più punti di uscita. Ogni punto iniziale viene verificato rispetto alle uscite disponibili e per ciascuno di essi viene determinato da Revit il percorso di uscita valido più vicino. Il nodo restituisce quindi il percorso con la distanza di percorrenza maggiore tra i risultati del percorso più breve.

Nell'esempio seguente, vengono innanzitutto creati un livello e una vista di pianta del pavimento per fornire la vista di Revit necessaria per il calcolo della traiettoria di movimento. Vengono generati diversi punti utilizzando `Point.ByCoordinates` che vengono combinati in un elenco che rappresenta le possibili posizioni di uscita. `PathOfTravel.LongestOfShortestExitPaths` utilizza la vista della pianta del pavimento e l'elenco di punti di destinazione per calcolare le traiettorie di movimento, restituendo il percorso con la distanza di percorrenza maggiore.
___
## File di esempio

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

## In profondità
Questo nodo acquisisce un'etichetta e ne modifica la posizione di testa. In questo modo è possibile automatizzare un comportamento di posizionamento coerente, in modo che le etichette siano direttamente sopra l'elemento a cui vengono assegnate.

In questo esempio, viene selezionata una porta nella vista “Studio Live Work Core B”. La posizione di tale porta viene estratta e quindi utilizzata come input originale in Tag.ByElementAndLocation insieme ai valori Boolean per horizontal e addLeader. La posizione originale viene modificata in modo che la posizione dell'etichetta non si sovrapponga direttamente all'elemento utilizzando il nodo Tag.SetHeadLocation.

___
## File di esempio

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

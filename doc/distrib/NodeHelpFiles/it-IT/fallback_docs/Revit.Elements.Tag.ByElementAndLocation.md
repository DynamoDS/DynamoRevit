## In profondità
Questo nodo assegna etichette agli elementi di Revit in base a view, element, location, horizontal (in caso contrario, l'etichetta verrà orientata in base all'elemento) e addLeader come input.

In questo esempio, viene selezionata una porta nella vista “Studio Live Work Core B”. La posizione di tale porta viene estratta e quindi utilizzata come input originale in Tag.ByElementAndLocation insieme ai valori Boolean per horizontal e addLeader. La posizione originale viene modificata in modo che la posizione dell'etichetta non si sovrapponga direttamente all'elemento utilizzando il nodo Tag.SetHeadLocation.

___
## File di esempio

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

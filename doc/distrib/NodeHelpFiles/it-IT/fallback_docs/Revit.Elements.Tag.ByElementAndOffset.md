## In profondità
Questo nodo assegna etichette agli elementi di Revit in base a view, element, offset, horizontalAligment, verticalAlignment, horizontal (in caso contrario, l'etichetta verrà orientata in base all'elemento) e addLeader come input.

In questo esempio, viene selezionata una porta nella vista “Studio Live Work Core B” e poi utilizzata come input in Tag.ByelementAndOffset. La posizione della porta viene estratta e utilizzata come punto iniziale del vettore. Lo stesso punto viene modificato tramite un dispositivo di scorrimento che cambia i punti x e y e utilizzato come punto finale del vettore. Questo vettore viene utilizzato come input offset insieme ai valori true negli input horizontal e addLeader. horizontalAlignment viene definito dai valori dell'elenco a discesa del nodo Selection Horizontal Text Alignment (Left, Center, Right) e dai valori dell'elenco a discesa del nodo Selection Vertical Text Alignment (Bottom, Middle, Top).

___
## File di esempio

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

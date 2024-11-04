## In profondità
`Element.Delete` funziona come l'opzione di eliminazione nell'interfaccia di Revit. Elimina l'elemento di input ed eventuali elementi che dipendono da esso.

Ad esempio, l'eliminazione di un muro con porte al suo interno comporterà l'eliminazione anche delle porte. L'output risultante è un elenco nidificato degli ID degli elementi eliminati. Nota: questo nodo è preferibile utilizzarlo nella modalità di esecuzione manuale in Dynamo.

Nell'esempio seguente, tutte le istanze di famiglia "Help Button" vengono eliminate dal documento (file) corrente.
___
## File di esempio

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

## In profondità
Questo nodo recupera la matrice di trasformazione inversa di un determinato elemento di collegamento di Revit. In Revit, i modelli collegati possono essere posizionati con trasformazioni (traslazione, rotazione, messa in scala). Questo nodo consente di ottenere l'inverso di tale trasformazione, riconvertendo efficacemente le coordinate dallo spazio del modello collegato nel sistema di coordinate del modello di Revit host.

In questo esempio, vengono selezionati tutti gli elementi collegati di Revit al livello L3 che sono poi utilizzati come input in LinkElement.LinkInverseTransform. L'output è il sistema di coordinate del modello di Revit host.
___
## File di esempio

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

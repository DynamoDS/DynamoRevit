## In profondità
Questo nodo recupera la matrice di trasformazione applicata ad un elemento di collegamento di Revit all'interno del modello host.
In altre parole, restituisce la trasformazione di posizione, rotazione e messa in scala che mappa il sistema di coordinate dell'elemento collegato nel sistema di coordinate del progetto di Revit host.
Ciò risulta utile quando è necessario allineare, analizzare o manipolare la geometria tra modelli collegati.

In questo esempio, vengono selezionati tutti gli elementi collegati di Revit al livello L3 che sono poi utilizzati come input in LinkElement.LinkTransform. L'output è la trasformazione di posizione, rotazione e messa in scala dell'elemento collegato.
___
## File di esempio

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

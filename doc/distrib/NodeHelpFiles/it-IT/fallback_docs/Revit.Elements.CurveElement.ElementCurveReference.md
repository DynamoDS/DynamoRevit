## In profondità
Questo nodo recupera il riferimento alla curva associato ad un determinato CurveElement di Revit, ad esempio una curva del modello o una linea di dettaglio. Il riferimento può quindi essere utilizzato come input in altri nodi che richiedono un riferimento geometrico, ad esempio quotatura, allineamento o creazione di percorsi divisi.

In questo esempio, viene creata una curva del modello utilizzando un punto iniziale e un punto finale, quindi inserita nel nodo CurveElement.ElementCurveReference. L'output è un riferimento geometrico dell'elemento curva che può essere utilizzato nelle operazioni a valle.
___
## File di esempio

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

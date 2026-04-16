## In profondità
Il nodo ReferencePlane.ByLine in Dynamo crea un piano di riferimento di Revit utilizzando una linea definita come base. Ciò consente di generare piani di riferimento personalizzati in posizioni e orientamenti specifici.

In questo esempio, vengono definiti due punti utilizzando Point.ByCoordinates con dispositivi di scorrimento regolabili. Viene quindi creato un nodo Line.ByStartPointEndPoint tra questi due punti e infine il nodo ReferencePlane.ByLine genera un piano di riferimento lungo tale linea.
___
## File di esempio

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

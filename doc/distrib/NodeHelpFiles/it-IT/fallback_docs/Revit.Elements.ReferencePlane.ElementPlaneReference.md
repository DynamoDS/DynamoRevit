## In profondità
Questo nodo estrae il riferimento effettivo all'elemento di Revit di un piano di riferimento selezionato. Ciò è utile quando è necessario utilizzare tale piano come riferimento host per la geometria o le quote all'interno di Revit.

Esempio:
In questo grafico, vengono definiti due punti utilizzando le coordinate e tra di essi viene creato un piano di riferimento con ReferencePlane.ByStartPointEndPoint. Tale piano di riferimento viene quindi collegato a ReferencePlane.ElementPlaneReference, che genera il riferimento nativo di Revit del piano, rendendolo pronto per essere utilizzato per attività di hosting o allineamento.
___
## File di esempio

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

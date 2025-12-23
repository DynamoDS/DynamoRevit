## In profondità
Questo nodo recupera il piano di riferimento associato ad un determinato elemento piano di disegno. Ciò consente di identificare o riutilizzare lo stesso piano di riferimento per creare o modificare la geometria.

In questo esempio, viene definito un piano che è poi connesso al nodo SketchPlane.ByPlane, che genera un piano di disegno corrispondente. Questo piano di disegno viene utilizzato come input in SketchPlane.ElementPlaneReference dove l'output può essere utilizzato per quotatura, allineamento, vincoli o altre operazioni che richiedono un riferimento di Revit.

___
## File di esempio

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)

## In profondità
`ElementFaceReference.BySurface` crea un riferimento alla superficie di Revit da una superficie selezionata o generata. Converte una superficie di Dynamo associata alla geometria di Revit in `ElementFaceReference`. Questo riferimento può quindi essere utilizzato da altri nodi che necessitano di un riferimento alla superficie di Revit, anziché solo della geometria di superficie.

Nell'esempio seguente, viene creato un muro e viene utilizzata una superficie di tale muro come input in `ElementFaceReference.BySurface`. L'output è `ElementFaceReference` che può essere utilizzato dai nodi che richiedono un riferimento ad una superficie dell'elemento di Revit.
___
## File di esempio

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

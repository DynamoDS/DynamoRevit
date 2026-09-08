## En detalle:
`ElementFaceReference.BySurface` crea una referencia de cara de Revit a partir de una superficie seleccionada o generada. Convierte una superficie de Dynamo asociada a la geometría de Revit en un elemento `ElementFaceReference`. A continuación, esta referencia pueden utilizarla otros nodos que necesiten una referencia de cara de Revit en lugar de solo la geometría de la superficie.

En el ejemplo siguiente, se crea un muro y se utiliza una superficie de ese muro como entrada para `ElementFaceReference.BySurface`. El resultado es un objeto `ElementFaceReference` que pueden utilizar los nodos que requieran una referencia a una cara de un elemento de Revit.
___
## Archivo de ejemplo

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

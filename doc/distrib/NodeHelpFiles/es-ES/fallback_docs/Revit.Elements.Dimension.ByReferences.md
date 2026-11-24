## En detalle:
`Dimension.ByReferences` coloca una cota a lo largo de la línea de entrada en la vista seleccionada mediante las referencias indicadas. El resultado es un elemento de cota de Revit.

En el ejemplo siguiente, se crean varios puntos y se conectan mediante nodos de línea para generar curvas de modelo en Revit. A continuación, las curvas de modelo se convierten en referencias de curva de Revit. Las referencias se agrupan en una lista y se utilizan como entrada `references` para `Dimension.ByReferences`. Se crean líneas adicionales para definir la ubicación y la dirección de las cotas. La vista activa de Revit también se proporciona como entrada `view`.
___
## Archivo de ejemplo

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

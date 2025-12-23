## En detalle:
Este nodo recupera la referencia de curva asociada con un CurveElement de Revit determinado, como una curva de modelo o una línea de detalle. A continuación, la referencia se puede utilizar como entrada para otros nodos que requieran una referencia de geometría, como la acotación, la alineación o la creación de caminos divididos.

En este ejemplo, se crea una curva de modelo mediante un punto inicial y un punto final y, a continuación, se introduce en el nodo CurveElement.ElementCurveReference. La salida es una referencia geométrica del elemento de curva que se puede utilizar en operaciones posteriores.
___
## Archivo de ejemplo

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

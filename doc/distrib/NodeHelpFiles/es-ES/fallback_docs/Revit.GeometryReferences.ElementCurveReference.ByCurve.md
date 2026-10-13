## En detalle:
`ElementCurveReference.ByCurve` crea una referencia de curva de elemento de Revit a partir de una curva de Dynamo. Una referencia de curva resulta útil cuando otro nodo de Revit necesita una curva que se pueda seleccionar o utilizar como referencia en lugar de limitarse a la geometría de Dynamo. Se suele emplear en flujos de trabajo que requieren referencias de Revit, como la creación de cotas, alineaciones, restricciones u otros elementos que deban hacer referencia a la geometría del modelo.

En el ejemplo siguiente, se selecciona una curva y se utiliza como entrada para `ElementCurveReference.ByCurve`. El resultado es un objeto `ElementCurveReference` que se utiliza a continuación como referencia de curva de Revit para los nodos posteriores que requieren referencias basadas en curvas.
___
## Archivo de ejemplo

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

## En detalle:
Este nodo recupera el plano de referencia asociado con un elemento de plano de boceto determinado. Esto ayuda a identificar o reutilizar el mismo plano de referencia para crear o modificar geometría.

En este ejemplo, se define un plano y, a continuación, se conecta al nodo SketchPlane.ByPlane, que genera el plano de boceto correspondiente. Este plano de boceto se utiliza como entrada para SketchPlane.ElementPlaneReference, donde el valor de salida se puede utilizar para la acotación, la alineación, la aplicación de restricciones u otras operaciones que requieran una referencia de Revit.

___
## Archivo de ejemplo

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)

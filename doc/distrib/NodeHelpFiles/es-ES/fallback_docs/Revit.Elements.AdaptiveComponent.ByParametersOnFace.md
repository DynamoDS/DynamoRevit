## En detalle:
Este nodo coloca componentes adaptativos aplicando valores de parámetros UV a una cara seleccionada para definir las ubicaciones de colocación del tipo de familia adaptativa.

En este ejemplo, se crea una superficie dentro de la familia de masas mediante la extrusión de una curva (esto se hace manualmente), y esa superficie se selecciona como entrada de cara. A continuación, se proporcionan los valores UV para determinar las posiciones de colocación, y se utiliza la familia  Diagnostic Tripod – 1 Point.rfa como tipo. El nodo AdaptiveComponent.ByParametersOnFace genera componentes adaptativos colocados en la cara seleccionada. Tenga en cuenta que "Diagnostic Tripod – 1 Point.rfa" debe cargarse en la familia de masas antes de ejecutar este gráfico.

Para ejecutar este archivo de ejemplo de ayuda del nodo, debe cargar "Diagnostics Tripod-1 point.rfa" en el archivo de Revit. La familia se almacena aquí. C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## Archivo de ejemplo

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)

## En detalle:
Este nodo evalúa una superficie especificada para determinar si es válida para su uso en la definición de un panel analítico. Una superficie válida suele ser plana, continua y adecuada para la conversión en una representación analítica dentro del entorno de modelo analítico de Revit.

En este ejemplo, se recopilan las caras de un elemento de losa del proyecto y se envía la cara superior al nodo como entrada. El nodo devuelve un resultado booleano que indica si la superficie seleccionada cumple los requisitos para crear un panel analítico, junto con un mensaje opcional que describe los problemas encontrados durante la validación.
___
## Archivo de ejemplo

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

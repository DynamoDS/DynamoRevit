## En detalle:
Este nodo toma un valor de longitud numérico y un identificador de tipo de unidad, convirtiendo el valor de entrada en las unidades de longitud del proyecto de Revit activo. La salida es un valor doble que representa el resultado convertido.

En este ejemplo, un control deslizante numérico proporciona un valor de longitud y se selecciona una unidad (por ejemplo, metros) para obtener su cadena Unit.TypeId. Ambos están conectados al nodo UnitsUtilities.ConvertToCurrentProjectLengthUnit, que devuelve el valor de longitud convertido según la configuración de unidades del proyecto.
___
## Archivo de ejemplo

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

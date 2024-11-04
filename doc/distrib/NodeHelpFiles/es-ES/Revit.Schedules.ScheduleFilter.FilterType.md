## En detalle:
`ScheduleFilter.FilterType` devuelve el método utilizado para el filtro de entrada.
Entre los posibles tipos de filtro, se incluyen los siguientes:

- Equal: el valor del campo es igual al valor especificado.
- NotEqual: el valor del campo no es igual al valor especificado.
- GreaterThan: el valor del campo es mayor que el valor especificado.
- GreaterThanOrEqual: el valor del campo es mayor o igual que el valor especificado.
- LessThan: el valor del campo es menor que el valor especificado.
- LessThanOrEqual: el valor del campo es menor o igual que el valor especificado.
- Contains: en un campo de cadena, el valor del campo contiene la cadena especificada.
- NotContains: en un campo de cadena, el valor del campo no contiene la cadena especificada.
- BeginsWith: en un campo de cadena, el valor del campo comienza por la cadena especificada.
- NotBeginsWith: en un campo de cadena, el valor del campo no comienza por la cadena especificada.
- EndsWith: en un campo de cadena, el valor del campo termina por la cadena especificada.
- NotEndsWith: en un campo de cadena, el valor del campo no termina por la cadena especificada.
- IsAssociatedWithGlobalParameter: el campo está asociado a un parámetro global especificado de un tipo compatible.
- IsNotAssociatedWithGlobalParameter: el campo no está asociado a un parámetro global especificado de un tipo compatible.

En el ejemplo siguiente, se recopila la primera tabla de planificación del archivo de Revit actual. A continuación, se comprueba si hay filtros en la vista de tabla de planificación y el único filtro aplicado es el tipo de filtro "La cadena no termina por".
___
## Archivo de ejemplo

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

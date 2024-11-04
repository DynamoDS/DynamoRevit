## En detalle:
`View.Partsvisibility` devuelve el parámetro de visibilidad de los elementos de piezas en la vista especificada. Las piezas de Revit son una forma de dividir familias del sistema en subcomponentes. Como resultado, existen diferentes métodos para mostrar los objetos.

Entre los valores de este nodo, se incluyen los siguientes:
- Unset: no se ha definido la visibilidad de las piezas para la vista.
- ShowPartsOnly: muestra solo las piezas de la vista.
- ShowOriginalOnly: muestra los elementos originales de la vista, pero no las piezas.
- ShowPartsAndOriginal: muestra tanto los elementos originales como las piezas de la vista.

En el ejemplo siguiente, se devuelve la opción de visibilidad de piezas de la vista activa.
___
## Archivo de ejemplo

![View.Partsvisibility](./Revit.Elements.Views.View.Partsvisibility_img.jpg)

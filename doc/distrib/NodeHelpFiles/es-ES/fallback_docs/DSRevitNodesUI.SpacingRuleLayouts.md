## En detalle:
`Spacing Rule Layout` recupera el diseño de la regla de espaciado a partir de un elemento de camino dividido. El camino dividido se utiliza como entrada y la salida es la regla de diseño de espaciado actual que controla cómo se distribuyen los puntos a lo largo del camino.

En el ejemplo siguiente, se selecciona una superficie dividida del modelo de Revit y se utiliza como entrada para modificar sus parámetros. Un nodo de cadena proporciona el nombre del parámetro "Layout", mientras que `Spacing Rule Layout` aporta el valor del tipo de regla de espaciado. El gráfico actualiza la regla de diseño de la superficie dividida a `FixedNumber`. El resultado es el elemento de superficie dividida modificado.
___
## Archivo de ejemplo

![Spacing Rule Layout](./DSRevitNodesUI.SpacingRuleLayouts_img.jpg)

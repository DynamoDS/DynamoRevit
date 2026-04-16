## En detalle:
Este nodo filtra los elementos de entrada en función de su ubicación vertical en el modelo. Permite elegir si se evalúa la coordenada Z superior o inferior (altura) de un elemento para realizar una comparación o filtrado modo lógico. Normalmente se utiliza como parte de un sistema de filtrado, a menudo junto con un ConnectionNode, para aislar elementos situados por encima o por debajo de determinadas elevaciones. Resulta útil en flujos de trabajo que impliquen análisis espaciales, como la separación de elementos de construcción por nivel o zona.

En este ejemplo, filtramos todos los elementos de datos de estructura seleccionados por su posición en la coordenada Z "superior" (altura). Esta información se puede utilizar a su vez para determinar si la posición es más baja que un nivel, un suelo o un techo.
___
## Archivo de ejemplo

![Filter.ByVerticalPosition](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByVerticalPosition_img.jpg)

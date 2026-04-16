## En detalle:
Este nodo devuelve una lista de nodos de conexión filtrados por la forma de sección seleccionada.

Al seleccionar un grupo de conexión formado por una forma en W y HSS, este ejemplo filtra la lista de nodos de conexión a aquellos que tienen una sección de ala del alma. Los elementos de datos de estructura pueden tener sublistas de elementos, por lo que el índice se establece en 0, el valor por defecto.

En el ejemplo, se seleccionan una forma en W y un HSS y el resultado consiste en una lista de diccionarios con las propiedades "aceptado" y "rechazado". El valor "aceptado" es el nodo de conexión filtrado por la forma de sección seleccionada. En este caso, la forma de sección es `W24X104`.
___
## Archivo de ejemplo

![Filter.IsItAWebFlangeSection](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.IsItAWebFlangeSection_img.jpg)

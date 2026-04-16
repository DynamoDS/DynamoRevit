## En detalle:
Este nodo devuelve una lista de nodos de conexión filtrados por un rango de grados mín. - máx. y el eje seleccionado.

En el ejemplo, se seleccionan un pilar en forma de W y una riostra HSS en ángulo y el resultado consiste en una lista de diccionarios con las propiedades "aceptado" y "rechazado". El valor "aceptado" es un nodo de conexión que se encuentra dentro del rango de 0 a 60 grados al comparar el eje X de ambos elementos. Si el valor máximo fuera de 45 grados, se rechazaría el nodo de conexión. Los parámetros `indexFirst` e `indexSecond` se establecen en "use levels" (usar niveles) para alinearse con la estructura de datos de entrada.
___
## Archivo de ejemplo

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

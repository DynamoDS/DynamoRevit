## En detalle:
Este nodo devuelve una lista de nodos de conexión filtrados por un rango de grados mín. - máx. del ángulo de inclinación. Este es el ángulo entre el eje X de un elemento de datos de estructura y el eje horizontal.

En el ejemplo, se seleccionan un pilar en forma de W y una riostra HSS en ángulo y el resultado consiste en una lista de diccionarios con las propiedades "aceptado" y "rechazado". El valor "aceptado" es un nodo de conexión que tiene un ángulo de inclinación entre 30 y 60 grados. La riostra HSS en ángulo tiene un ángulo de inclinación de ~60 grados, por lo que se acepta. El pilar en forma de W tiene un ángulo de inclinación de 90 grados y se rechaza.
___
## Archivo de ejemplo

![Filter.BySlopeAngle](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySlopeAngle_img.jpg)

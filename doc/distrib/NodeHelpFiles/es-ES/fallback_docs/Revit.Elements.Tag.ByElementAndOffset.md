## En detalle:
Este nodo etiqueta los elementos de Revit en función de una vista, un elemento, un desfase, horizontalAligment, verticalAlignment, una horizontal (si no, la etiqueta se orientará en función del elemento) y addLeader como entradas.

En este ejemplo, se selecciona una puerta en la vista "Studio Live Work Core B" y se utiliza como entrada para Tag.ByElementAndOffset. Se extrae la ubicación de esa puerta y se utiliza como punto inicial del vector. El mismo punto se modifica con un control deslizante que cambia los puntos X e Y y se utiliza como punto final del vector. Este vector se utiliza como entrada para el desfase junto con los valores verdaderos (true) en las entradas de horizontal y addLeader. horizontalAlignment se define mediante los valores desplegables del nodo Selection Horizontal Text Alignment (izquierda, centro y derecha) y los valores desplegables del nodo Selection Vertical Text Alignment (inferior, intermedia y superior).

___
## Archivo de ejemplo

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

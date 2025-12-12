## En detalle:
Este nodo recupera la matriz de transformación aplicada a un elemento de vínculo de Revit en el modelo anfitrión.
En otras palabras, devuelve la transformación de posición, rotación y escala que el sistema de coordenadas del elemento vinculado asigna al sistema de coordenadas del proyecto anfitrión de Revit.
Esto resulta útil cuando se necesita alinear, analizar o manipular la geometría entre modelos vinculados.

En este ejemplo, se seleccionan todos los elementos vinculados de Revit en el nivel L3 y se introducen en LinkElement.LinkTransform. La salida es la transformación de posición, rotación y escala del elemento vinculado.
___
## Archivo de ejemplo

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

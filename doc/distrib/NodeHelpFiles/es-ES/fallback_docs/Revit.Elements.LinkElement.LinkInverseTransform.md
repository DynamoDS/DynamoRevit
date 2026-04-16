## En detalle:
Este nodo recupera la matriz de transformación inversa de un elemento de vínculo de Revit especificado. En Revit, los modelos vinculados se pueden colocar con transformaciones (traslación, rotación y escalado). Este nodo permite obtener la transformación inversa y convertir de forma eficaz las coordenadas del espacio del modelo vinculado al sistema de coordenadas del modelo anfitrión de Revit.

En este ejemplo, se seleccionan todos los elementos vinculados de Revit en el nivel L3 y se introducen en LinkElement.LinkInverseTransform. La salida es el sistema de coordenadas del modelo anfitrión de Revit.
___
## Archivo de ejemplo

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

## En detalle:
Este nodo proyecta un rayo en un modelo vinculado de Revit desde un origen y una dirección especificados y, a continuación, traza sus rebotes sucesivos en los elementos vinculados. Cada rebote representa un punto en el que el rayo interseca la geometría del modelo vinculado, hasta alcanzar un número máximo definido de reflejos.

En este ejemplo, se selecciona un elemento vinculado y la ubicación de ese elemento se utiliza como entrada de origen para LinkElement.ByRayBounce junto con una dirección, maxBounces y una vista. Las salidas son puntos y elementos vinculados.
___
## Archivo de ejemplo

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)

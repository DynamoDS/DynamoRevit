## En detalle:
Este nodo crea una vista de alzado a partir de un ElevationMarker existente mediante la especificación del índice del marcador. Cada ElevationMarker de Revit puede alojar hasta cuatro vistas de alzado individuales, una por cada dirección (norte, sur, este y oeste). Este nodo permite generar una de esas elevaciones direccionales haciendo referencia al marcador y al número de índice deseado.

En este ejemplo, se crea un marcador de alzado y se utiliza como entrada elevationMarker para el nodo ElevationMarker.CreateElevationByMarkerIndex, junto con una vista y un índice (0,1,2,3).

___
## Archivo de ejemplo

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

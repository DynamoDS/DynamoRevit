## En detalle:
`PathOfTravel.LongestOfShortestExitPaths` crea un camino del recorrido que representa la ruta más larga de entre las más cortas desde un grupo de puntos hasta uno o varios puntos de salida. Se comprueba cada punto de inicio con respecto a las salidas disponibles y Revit determina el camino de salida válido más cercano para cada uno de ellos. A continuación, el nodo devuelve el camino con la mayor distancia de recorrido de entre los resultados de los caminos más cortos.

En el ejemplo siguiente, se crean primero una vista de nivel y otra de plano de planta para proporcionar la vista de Revit necesaria para el cálculo del camino del recorrido. Se generan varios puntos mediante `Point.ByCoordinates` y se agrupan en una lista que representa las posibles ubicaciones de salida. `PathOfTravel.LongestOfShortestExitPaths` utiliza la vista de plano de planta y la lista de puntos de destino para calcular los caminos del recorrido y devuelve el camino con la mayor distancia de recorrido.
___
## Archivo de ejemplo

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

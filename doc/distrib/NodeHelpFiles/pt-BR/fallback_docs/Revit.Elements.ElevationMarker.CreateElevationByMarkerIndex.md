## Em profundidade
Esse nó cria uma vista de elevação com base em um ElevationMarker existente especificando o índice do marcador. Cada ElevationMarker no Revit pode hospedar até quatro vistas de elevação individuais, uma para cada direção (norte, sul, leste e oeste). Esse nó permite gerar uma dessas elevações direcionais ao referenciar o marcador e o número de índice desejado.

Neste exemplo, é criado um marcador de elevação e usado como elevationMarker de entrada para o nó ElevationMarker.CreateElevationByMarkerIndex junto com uma vista e um índice (0,1,2,3).

___
## Arquivo de exemplo

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

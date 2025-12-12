## Em profundidade
Esse nó executa uma análise de reflexo de raio no modelo do Revit. Partindo de um determinado ponto de origem e viajando ao longo de um vetor de direção especificado, ele traça o caminho do raio conforme ele efetua a interseção com elementos no modelo. Quando o raio atinge uma superfície, ele pode continuar refletindo nessa superfície, dependendo do número de reflexões permitidas, simulando luz, visibilidade ou comportamento de reflexão do caminho.

Neste exemplo, é selecionado um elemento e sua localização é usada para origem de entrada para o nó RayBounce.ByOriginDirection, junto com uma direção, maxBounces e uma vista.
___
## Arquivo de exemplo

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)

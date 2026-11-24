## Em profundidade
`PathOfTravel.LongestOfShortestExitPaths` cria um caminho de deslocamento que representa a rota mais longa entre as mais curtas de um grupo de pontos para um ou mais pontos de saída. Cada ponto inicial é testado em relação às saídas disponíveis, e o Revit determina o caminho de saída válido mais próximo para cada uma. Em seguida, o nó retorna o caminho com a maior distância de deslocamento entre os resultados do caminho mais curto.

No exemplo abaixo, um nível e uma vista de planta de piso são criados primeiro para fornecer a vista do Revit necessária para o cálculo do caminho de deslocamento. Vários pontos são gerados usando `Point.ByCoordinates` e combinados em uma lista que representa as possíveis localizações de saída. `PathOfTravel.LongestOfShortestExitPaths` usa a vista de planta baixa e a lista de pontos de destino para calcular os caminhos de deslocamento, retornando o caminho com a maior distância de deslocamento.
___
## Arquivo de exemplo

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

## Em profundidade
Esse nó identifica os elementos do Revit com uma vista, um elemento, um deslocamento, um alinhamento horizontal, um alinhamento vertical, horizontal (caso contrário, o identificador orientará com base no elemento) e addLeader como entradas.

Neste exemplo, é selecionada uma porta na vista “Studio Live Work Core B” e é usada como entrada para Tag.ByelementAndOffset. A localização dessa porta é extraída e usada como o ponto inicial do vetor. O mesmo ponto é modificado usando uma barra deslizante que altera os pontos x e y e usados como ponto final do vetor. Esse vetor é usado como entrada para deslocamento junto com valores true nas entradas horizontal e addLeader. O horizontalAlignment é definido pelos valores do menu suspenso do nó Alinhamento do texto horizontal de seleção (Esquerdo, Centro, Direito) e pelos valores do menu suspenso do nó Alinhamento do texto vertical de seleção (Inferior, Meio, Superior).

___
## Arquivo de exemplo

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

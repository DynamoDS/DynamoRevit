## Em profundidade
Esse nó retorna uma lista de nós de conexão filtrados por um intervalo de graus mín.-máx. e pelo eixo selecionado.

No exemplo, uma coluna em forma de W e um suporte HSS angulado são selecionados, e a saída consiste em uma lista de dicionários com propriedades “aceito” e “rejeitado”. O valor aceito é um nó de conexão que está dentro do intervalo de 0 a 60 graus ao comparar o eixo X de ambos os elementos. Se o valor máximo fosse 45 graus, o nó de conexão seria rejeitado. Os parâmetros `indexFirst` e `indexSecond` são definidos como “níveis de uso” para se alinharem com a estrutura de dados de entrada.
___
## Arquivo de exemplo

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

## Em profundidade
Esse nó filtra os elementos de entrada com base em seu posicionamento vertical no modelo. Ele permite que você escolha se deseja avaliar a coordenada Z superior ou inferior (altura) de um elemento para comparação ou filtragem lógica. Normalmente, esse nó é usado como parte de um sistema de filtragem, frequentemente em conjunto com um ConnectionNode, para isolar elementos acima ou abaixo de determinadas elevações. Ele é útil em fluxos de trabalho que envolvem análise espacial, como a separação de elementos de construção por nível ou zona.

Neste exemplo, estamos filtrando todos os itens de dados de estrutura selecionados por sua posição (altura) da coordenada z “Superior”. Isso pode ser usado para determinar se a posição é mais baixa que um nível, piso ou forro.
___
## Arquivo de exemplo

![Filter.ByVerticalPosition](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByVerticalPosition_img.jpg)

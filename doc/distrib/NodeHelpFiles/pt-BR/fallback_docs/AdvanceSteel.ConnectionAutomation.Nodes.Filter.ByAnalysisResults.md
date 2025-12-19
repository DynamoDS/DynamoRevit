## Em profundidade
Esse nó filtra uma lista de ConnectionNodes verificando se o valor da força em um índice especificado está dentro de um intervalo definido. Os dados de força vêm dos resultados da análise estrutural ou do modelo analítico do Revit e são filtrados pelo tipo de resultado selecionado (por exemplo, Fx, Fy, Fz, Mx, My, Mz).

Neste exemplo, um conjunto de elementos de coluna é selecionado e avaliado com base no componente de força Fz, usando o resultado da análise escolhido e o caso de carga. Somente os elementos cujo valor Fz está dentro do intervalo de força especificado são retornados como conexões aceitas.
___
## Arquivo de exemplo

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

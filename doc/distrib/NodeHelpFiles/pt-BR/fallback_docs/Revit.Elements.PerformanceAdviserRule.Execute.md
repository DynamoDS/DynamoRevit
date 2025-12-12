## Em profundidade
Esse nó executa uma regra específica do Consultor de desempenho em relação a um conjunto de elementos do Revit.

Neste exemplo, a regra do Consultor de desempenho verifica se “O corte de vista está desativado”. Os resultados são passados para o nó FailureMessage.FailingElements, que gera os elementos específicos no modelo que falharam nessa verificação. Esse fluxo de trabalho torna mais fácil para os usuários rastrear e corrigir os elementos exatos responsáveis pelos problemas.

___
## Arquivo de exemplo

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)

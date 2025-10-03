## Em profundidade

Retorna o tipo de unidade de um parâmetro.

No exemplo abaixo, estamos extraindo todos os parâmetros do elemento e usando-os como entrada. O objetivo é exibir o tipo de unidade de cada parâmetro.
Por exemplo, se Element.Parameters mostrar Diâmetro base: 1' – 3 ¼"”, a saída de Parameter.Unit será “Unidade (Nome = pés ou metros)”.
Se um parâmetro não tiver uma unidade (como Element.Parameters = Aplicar corte: Não), Parameter.Unit retornará nulo.
Como o Dynamo não possui unidades, é crucial identificar os tipos de unidades recebidas para realizar cálculos precisos. Esse nó ajuda o Dynamo a reconhecer e processar os dados dessas unidades.

___
## Arquivo de exemplo

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

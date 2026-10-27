## Em profundidade
`VectorAnalysisDisplay.ByViewPointsAndVectorValues` cria uma exibição da análise vetorial em uma vista do Revit. A vista de entrada define onde será apresentada a exibição da análise. Os pontos definem as localizações dos marcadores de análise, os vetores definem a direção da análise exibida e os valores definem o resultado numérico associado a cada ponto. A saída é um elemento de exibição da análise do Revit mostrado na vista selecionada.

No exemplo abaixo, são criados dois pontos para definir as localizações de amostra em uma vista 3D do Revit. Em seguida, são criados dois vetores para definir os dados da análise direcional nessas localizações. Os pontos e os vetores são combinados em listas e usados como entradas para `VectorAnalysisDisplay.ByViewPointsAndVectorValues` junto com uma vista, nome de análise e descrição. A saída é uma exibição da análise vetorial mostrada na vista selecionada do Revit.
___
## Arquivo de exemplo

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)

## Em profundidade
`ElementCurveReference.ByCurve` cria uma referência de curva de elemento do Revit com base em uma curva do Dynamo. Uma referência de curva é útil quando outro nó do Revit precisa de uma curva selecionável ou referenciável, em vez de apenas geometria do Dynamo. Isso é normalmente usado para fluxos de trabalho que requerem referências do Revit, como a criação de cotas, alinhamentos, restrições ou outros elementos que precisam referenciar a geometria do modelo.

No exemplo abaixo, é selecionada uma curva e usada como entrada para `ElementCurveReference.ByCurve`. A saída é uma `ElementCurveReference` que é usada como uma referência de curva do Revit para nós a jusante que exigem referências com base em curva.
___
## Arquivo de exemplo

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

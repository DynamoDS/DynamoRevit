## Em profundidade
Esse nó extrai a referência real do elemento do Revit de um plano de referência selecionado. Isso é útil quando você precisa usar esse plano como uma referência de hospedagem para a geometria ou dimensões dentro do Revit.

Exemplo:
Neste gráfico, são definidos dois pontos usando coordenadas e é criado um plano de referência entre eles com ReferencePlane.ByStartPointEndPoint. Em seguida, esse plano de referência é conectado ao ReferencePlane.ElementPlaneReference, que gera a referência nativa do Revit do plano, tornando-o pronto para ser usado nas tarefas de hospedagem ou alinhamento.
___
## Arquivo de exemplo

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

## Em profundidade
O nó ReferencePlane.ByLine no Dynamo cria um plano de referência do Revit usando uma linha definida como base. Isso permite gerar planos de referência personalizados em posições e orientações específicas.

Neste exemplo, são definidos dois pontos usando Point.ByCoordinates com controles deslizantes ajustáveis. Em seguida, é criado um Line.ByStartPointEndPoint entre esses dois pontos e, por fim, o nó ReferencePlane.ByLine gera um plano de referência ao longo dessa linha.
___
## Arquivo de exemplo

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

## Em profundidade
`ReferencePoint.ByPoint` cria um elemento de ponto de referência no documento da família do Revit ativo na localização do ponto especificado. Observação: O documento da família deve ser um componente adaptativo ou uma família de massa. Esse nó difere de `ReferencePoint.ByCoordinates`, pois usa um ponto do Dynamo para a localização. Isso é útil porque o usuário final pode usar a manipulação direta para editar o ponto do Dynamo, resultando na atualização do ponto de referência também. Para obter mais informações sobre manipulação direta, consulte [link](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

No exemplo abaixo, é criado um novo ponto de referência nas coordenadas 0,0,1.
___
## Arquivo de exemplo

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

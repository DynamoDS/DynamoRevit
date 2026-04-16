## Em profundidade
Esse nó avalia uma determinada superfície para determinar se ela é válida para uso na definição de um painel analítico. Uma superfície válida é normalmente plana, contínua e adequada para ser convertida em uma representação analítica no ambiente do modelo analítico do Revit.

Neste exemplo, as faces de um elemento de laje do projeto são coletadas e a face superior é fornecida ao nó como entrada. O nó retorna um resultado booleano que indica se a superfície selecionada atende aos requisitos para a criação de um painel analítico, juntamente com uma mensagem opcional que descreve todos os problemas encontrados durante a validação.
___
## Arquivo de exemplo

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

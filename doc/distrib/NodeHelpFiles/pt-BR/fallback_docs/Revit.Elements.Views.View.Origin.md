## Em profundidade
Cada vista no Revit tem uma origem. `View.Origin` retorna esse valor como um ponto do Dynamo. De acordo com a documentação da API do Revit, “A origem de uma vista de planta não é significativa”. Com isso em mente, o valor oferecido por `View.Origin` depende do usuário final.

No exemplo abaixo, é retornada a origem da vista ativa e uma vista 3D selecionada.
___
## Arquivo de exemplo

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)

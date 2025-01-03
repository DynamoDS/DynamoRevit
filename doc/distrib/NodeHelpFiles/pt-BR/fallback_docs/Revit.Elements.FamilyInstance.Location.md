## Em profundidade
`FamilyInstance.Location` retorna um ponto do Dynamo para a instância de família fornecida. Se não existir nenhuma localização, será retornado um valor nulo. `FamilyInstance.Location` funciona em um elemento com base em ponto e não retornará nenhuma localização para o elemento com base na curva no Revit, _por exemplo, uma parede ou elemento de viga_.

No exemplo abaixo, são coletadas todas as instâncias da família de portas na vista atual do documento atual. As localizações das portas são retornadas com `FamilyInstance.Location`.

No caso deste exemplo, as portas de parede cortina estão retornando um valor nulo. As localizações dos painéis cortina estão disponíveis por meio dos nós do painel cortina.
___
## Arquivo de exemplo

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

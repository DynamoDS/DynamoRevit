## Em profundidade
`Dimension.ValueOverride` retornará a substituição de valor da cota especificada se ela tiver um valor substituído. Para cotas multissegmentadas, será retornada uma lista aninhada de valores. Se a cota não tiver um valor substituído,será retornado um valor nulo.

No exemplo abaixo, a primeira cota é coletada da vista ativa e verificada se tem uma cota substituída. Se a cota for substituída, ela será apagada com o nó `Dimension.SetValueOverride`.
___
## Arquivo de exemplo

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)

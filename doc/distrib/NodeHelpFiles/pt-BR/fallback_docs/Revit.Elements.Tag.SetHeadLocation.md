## Em profundidade
Este nó recebe um identificador e altera sua posição no cabeçalho. Isso nos permite automatizar um comportamento de colocação consistente, de modo que os identificadores fiquem diretamente acima do elemento que estão identificando.

Neste exemplo, é selecionada uma porta na vista “Studio Live Work Core B”. A localização dessa porta é extraída e usada como a entrada original para Tag.ByElementAndLocation junto com valores booleanos para horizontal e addLeader. A localização original é modificada para que a localização do identificador não se sobreponha diretamente à parte superior do elemento usando o nó Tag.SetHeadLocation.

___
## Arquivo de exemplo

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

## Em profundidade
Esse nó identifica os elementos do Revit com uma vista, um elemento, uma localização, horizontal (caso contrário, o identificador orientará com base no elemento) e addLeader como entradas.

Neste exemplo, é selecionada uma porta na vista “Studio Live Work Core B”. A localização dessa porta é extraída e usada como a entrada original para Tag.ByElementAndLocation junto com valores booleanos para horizontal e addLeader. A localização original é modificada para que a localização do identificador não se sobreponha diretamente à parte superior do elemento usando o nó Tag.SetHeadLocation.

___
## Arquivo de exemplo

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

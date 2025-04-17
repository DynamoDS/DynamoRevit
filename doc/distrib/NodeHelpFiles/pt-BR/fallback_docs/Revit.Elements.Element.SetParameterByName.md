## Em profundidade
`Element.SetParameterByName` define um elemento de parâmetro (encontrado por nome) para um determinado valor. Os valores incluem: double, integer, boolean, ElementId, Element e string. No Revit, os parâmetros podem compartilhar o mesmo nome. Como resultado, o nó `Element.SetParameterByName` definirá o valor no primeiro parâmetro encontrado, classificado em ordem alfabética por UniqueId.

No exemplo abaixo, o parâmetro Comentários está sendo definido para todos os itens de mobiliário no modelo que estão localizados dentro de um ambiente. O valor do parâmetro Comentários é o nome do ambiente que é obtido.
___
## Arquivo de exemplo

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

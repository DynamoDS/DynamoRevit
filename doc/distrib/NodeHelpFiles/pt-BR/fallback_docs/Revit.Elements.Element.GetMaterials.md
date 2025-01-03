## Em profundidade
`Element.GetMaterials` retorna todos os materiais _(e suas IDs)_ que existem em um elemento do Revit. Os elementos com vários materiais retornarão uma lista para cada elemento. O parâmetro “paintMaterials” é um botão de alternância booleano para instruir o nó a coletar também materiais que são pintados pelo usuário.

No exemplo abaixo, são retornados os materiais de todas as instâncias de parede no documento (arquivo) atual.
___
## Arquivo de exemplo

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)

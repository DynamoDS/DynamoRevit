## Em profundidade
`Space.IsPointInsideSpace` verifica se um determinado ponto está dentro de um determinado espaço. Isso pode ser útil ao atribuir valores de marca a elementos dentro do Revit.

No exemplo abaixo, são coletados todos os terminais de ar na vista fornecida no documento Revit atual. Em seguida, suas localizações de ponto são comparadas aos espaços na vista fornecida com `Space.IsPointInsideSpace`. Usando o gerenciamento de listas, são desenvolvidas sublistas para filtrar os terminais de ar que ocorrem dentro dos espaços. Para obter mais informações sobre como usar List@Level, consulte [artigo](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Arquivo de exemplo

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

## Em profundidade
Coloca grupos do Revit com localização (pontos) e groupType como entradas. A saída Object.Type do groupType é Revit.Elements.ElementType

 No exemplo abaixo, todos os grupos de modelos são coletados do documento do Revit ativo. Os tipos dos grupos são retornados com Group.GroupType, reduzidos ao primeiro grupo na lista de projetos e usados como entrada para GroupType. A entrada de localização é colocada em 100,100.
___
## Arquivo de exemplo

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)

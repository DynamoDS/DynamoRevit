## Em profundidade
`GroupType.ByTypeId` é usado principalmente em fluxos de trabalho de criação e gerenciamento de parâmetros, nos quais um grupo de parâmetros do Revit deve ser especificado de forma programática. A entrada é uma sequência de caracteres que representa uma ID de tipo de grupo de parâmetros do Revit. A saída é um objeto `GroupType` que pode ser usado a jusante em nós que exigem uma classificação de agrupamento de parâmetros.

No exemplo abaixo, são criadas diversas sequências representando as IDs de tipo do grupo de parâmetros do Revit e combinadas em uma lista. A lista é usada como entrada para `GroupType.ByTypeId`. A saída é uma lista de objetos `GroupType` do Revit que representam os grupos de parâmetros correspondentes.
___
## Arquivo de exemplo

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)

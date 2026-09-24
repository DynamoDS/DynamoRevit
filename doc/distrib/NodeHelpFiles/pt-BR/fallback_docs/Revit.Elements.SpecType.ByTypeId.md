## Em profundidade
`SpecType.ByTypeId` converte uma sequência de caracteres de TypeId de especificação do Revit em um objeto `SpecType`. Os tipos de especificações definem o tipo de dados que um parâmetro representa no Revit, como comprimento, área, ângulo, material, força, texto, número inteiro e muitos outros. Isso é normalmente usado ao criar parâmetros compartilhados, parâmetros de projeto ou definições de parâmetro em versões mais recentes do Revit, nas quais a Autodesk substituiu o antigo sistema `ParameterType` por TypeIds do Forge.

No exemplo abaixo, uma sequência de caracteres que representa um TypeId de especificação do Revit é fornecida para `SpecType.ByTypeId`. A saída é um objeto `SpecType` que pode ser usado a jusante ao criar uma nova definição de parâmetro para que o Revit compreenda o tipo de dados que o parâmetro deve armazenar.
___
## Arquivo de exemplo

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

## Em profundidade
Similar a `Revit.Elements.FamilyType.ByFamilyAndName`, `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` retorna a definição de tipo de família do documento atual (se disponível). Isso é semelhante a `Revit.Elements.FamilyType.ByFamilyAndName`. No entanto, em vez de usar uma definição de família, esse nó depende da entrada de sequência de caracteres para ambos os valores. Se o tipo de família não estiver disponível no documento atual, será retornado um valor nulo.

No exemplo abaixo, é retornado um tipo de família de portas, “36" x 84”, da família “Door-Passage-Single-Flush”.
___
## Arquivo de exemplo

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)

## Em profundidade
`FamilyType.ByName` tentará recuperar o tipo de família fornecido do nome especificado no documento atual. Se o tipo de família não estiver disponível no documento atual, será retornado um valor nulo.

Observação: `FamilyType.ByName` pesquisa as definições de tipo de família na ordem de criação da família principal (por ID de elemento). Se várias famílias principais tiverem uma definição de tipo com o mesmo nome, será retornada a primeira encontrada. Para uma pesquisa mais concisa de tipos de família, use `FamilyType.ByFamilyAndName` ou `FamilyType.ByFamilyNameAndTypeName`

No exemplo abaixo, é retornado um tipo de família de portas, “36" x 84”, da família “Door-Passage-Single-Flush”.
___
## Arquivo de exemplo

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)

## Em profundidade
`ScheduleFilter.FilterType` retorna o método usado para o filtro de entrada.
Os tipos de filtro possíveis incluem:

- Igual – O valor do campo é igual ao valor especificado.
- Não é igual – O valor do campo não é igual ao valor especificado.
- Maior que – O valor do campo é maior que o valor especificado.
- Maior ou igual a – O valor do campo é maior ou igual ao valor especificado.
- Menor que – O valor do campo é menor que o valor especificado.
- Menor ou igual a – O valor do campo é menor ou igual ao valor especificado.
- Contém – Em um campo de sequência de caracteres, o valor do campo contém a sequência de caracteres especificada.
- Não contém – Em um campo de sequência de caracteres, o valor do campo não contém a sequência de caracteres especificada.
- Começa com – Em um campo de sequência de caracteres, o valor do campo começa com a sequência de caracteres especificada.
- Não começa com –Em um campo de sequência de caracteres, o valor do campo não começa com a sequência de caracteres especificada.
- Termina com –Em um campo de sequência de caracteres, o valor do campo termina com uma sequência de caracteres especificada.
- Não termina com – Em um campo de sequência de caracteres, o valor do campo não termina com a sequência de caracteres especificada.
- Está associado ao parâmetro global – O campo está associado a um parâmetro global especificado de um tipo compatível
- Não está associado ao parâmetro global – O campo não está associado a um parâmetro global especificado de um tipo compatível

No exemplo abaixo, é coletada a primeira tabela do arquivo atual do Revit. Em seguida, a vista da tabela é verificada quanto a filtros. O único filtro aplicado é um tipo de filtro “sequência de caracteres não termina com”.
___
## Arquivo de exemplo

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

## Em profundidade
Retorna se um determinado elemento de grupo no projeto do Revit está anexado a um grupo principal. Em termos mais simples, ele verifica se um grupo faz parte de um grupo maior aninhado. Esse nó é útil quando você precisa identificar e gerenciar grupos organizados dentro de outros grupos. Por exemplo, você pode usá-lo para:
• Filtrar grupos: isole grupos que não fazem parte de outros grupos.
• Gerenciar grupos aninhados: compreenda a estrutura dos grupos aninhados e processe-os adequadamente.
• Controlar a modificação de elementos: evite fazer alterações diretas em elementos dentro de um grupo que esteja anexado a um grupo principal, pois isso pode interromper a estrutura do grupo principal.
• Automatizar tarefas: gerencie e manipule grupos dinamicamente com base em se eles estão anexados ou não.
Essencialmente, o nó Group.IsAttached ajuda a entender o relacionamento entre grupos no modelo do Revit e a criar fluxos de trabalho do Dynamo que levem em conta essa hierarquia.

No exemplo abaixo, todos os grupos de modelos são coletados do documento do Revit ativo como entrada. As saídas são True ou False. Resultados True indicam que o grupo possui anexos (aninhamento). Resultados False indicam que o grupo NÃO possui anexos (aninhamento).

___
## Arquivo de exemplo

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)

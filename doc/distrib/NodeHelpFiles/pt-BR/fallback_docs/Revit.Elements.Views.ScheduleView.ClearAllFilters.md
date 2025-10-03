## Em profundidade
Remove todos os filtros definidos de uma determinada entrada de um ScheduleView.

Neste exemplo, definimos inicialmente que uma vista de tabela é roteada diretamente para ScheduleView.ScheduleFilters para mostrar explicitamente seus filtros de vista preexistentes. Para destacar a transformação, essa mesma vista passa por uma breve pausa de 10 milissegundos. Após esse breve atraso, a vista prossegue para o nó ScheduleView.ClearAllFilters, que remove todos os filtros aplicados. Por fim, a vista, agora modificada, é redirecionada de volta para outro nó ScheduleView.ScheduleFilters, demonstrando inequivocamente que sua lista de filtros se tornou uma lista vazia, confirmando assim a limpeza bem-sucedida de todos os filtros originais.
___
## Arquivo de exemplo

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

## Em profundidade
Ao coletar elementos no Revit com um coletor de categorias, é possível coletar famílias aninhadas que são compartilhadas. `Element.GetParentElement` ajuda a identificar se uma determinada instância de família está aninhada ao identificar seu elemento principal.

No exemplo abaixo, todas as instâncias da família “Cadeira-Breuer” são agrupadas por sua instância da família principal.
___
## Arquivo de exemplo

![Element.GetParentElement](./Revit.Elements.Element.GetParentElement_img.jpg)

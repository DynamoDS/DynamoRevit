## Em profundidade
`Element.Delete` funciona da mesma forma que a opção de exclusão na interface do Revit. Ele excluirá o elemento de entrada e todos os elementos que dependam dele.

Por exemplo, a exclusão de uma parede com portas também excluirá as portas. A saída consiste em uma lista aninhada das IDs dos elementos que foram excluídos como resultado. Observação: Esse nó é melhor usado no modo de execução manual no Dynamo.

No exemplo abaixo, todas as instâncias da família “Botão de ajuda” são excluídas do documento (arquivo) atual.
___
## Arquivo de exemplo

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

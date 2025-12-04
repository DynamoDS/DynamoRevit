## Em profundidade
O nó Revision.Issued no Dynamo é usado para verificar se uma revisão no Revit está marcada como emitida. Ele retorna um valor true ou false (booleano), ajudando as equipes a verificar rapidamente o status das revisões sem abrir as configurações de revisão do Revit.

Neste gráfico, o nó Selecionar revisão é usado para escolher uma revisão do projeto. O nó Revision.Issued verifica se a revisão selecionada foi emitida, e o resultado é exibido no nó Watch como true ou false. Isso facilita a confirmação do status do problema de uma revisão diretamente no Dynamo.

___
## Arquivo de exemplo

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)

## Em profundidade
O nó Revision.SetIssued no Dynamo permite controlar se uma revisão selecionada no Revit é marcada como emitida ou não emitida. Ele usa um elemento de revisão e uma entrada booleana (True/False), dando aos usuários controle direto sobre o status da revisão sem editá-la manualmente no Revit.
Neste gráfico, o nó Selecionar revisão é usado para selecionar uma revisão específica (por exemplo, “Seq. 1 – Projeto esquemático”). O nó Booleano fornece um valor True/False, que é conectado ao nó Revision.SetIssued para atualizar automaticamente o status de emissão da revisão.

___
## Arquivo de exemplo

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

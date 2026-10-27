## Em profundidade
`Transaction.Start` inicia uma transação no documento atual do Revit. As transações são usadas para agrupar as alterações feitas no modelo do Revit para que possam ser confirmadas ou revertidas juntas. `Transaction.Start` é normalmente usado com `Transaction.End` quando um fluxo de trabalho do Dynamo precisa de controle explícito sobre quando as alterações no documento do Revit começam e terminam.

No exemplo abaixo, é criado um nível e usado como entrada para `Transaction.Start`. Isso inicia uma transação do Revit antes que as alterações do modelo sejam feitas. Após as operações necessárias do Revit serem concluídas, `Transaction.End` fecha a transação e confirma as alterações no documento. A saída é um objeto de transação que pode ser passado para outros nós relacionados à transação.
___
## Arquivo de exemplo

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

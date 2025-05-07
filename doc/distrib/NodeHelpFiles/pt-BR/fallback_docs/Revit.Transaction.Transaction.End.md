## Em profundidade
`Transaction.End` conclui a transação do Dynamo atual e retorna o elemento especificado. No Revit, as transações representam alterações feitas no documento do Revit. Quando ocorre uma alteração, ela pode ser vista pelo botão de desfazer ativado. Usando `Transaction.End`, os usuários podem adicionar etapas ao gráfico do Dynamo, criando uma ação de desfazer para cada etapa em que `Transaction.End` é usado.

No exemplo abaixo, é colocada uma instância de família no documento do Revit. `Transaction.End` é chamado para concluir a colocação antes de rotacionar a instância de família com `FamilyInstance.SetRotation`.

___
## Arquivo de exemplo

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

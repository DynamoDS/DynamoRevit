## In profondità
`Transaction.Start` avvia una transazione nel documento di Revit corrente. Le transazioni vengono utilizzate per raggruppare le modifiche apportate al modello di Revit in modo che sia possibile eseguirne il commit o il rollback insieme. `Transaction.Start` viene in genere utilizzato con `Transaction.End` quando un workflow di Dynamo richiede un controllo esplicito sull'inizio e sulla fine delle modifiche al documento di Revit.

Nell'esempio seguente, viene creato un livello che viene utilizzato come input in `Transaction.Start`. Viene avviata una transazione di Revit prima che vengano apportate modifiche al modello. Al termine delle operazioni di Revit richieste, `Transaction.End` chiude la transazione ed esegue il commit delle modifiche al documento. L'output è un oggetto transazione che può essere trasmesso ad altri nodi correlati alla transazione.
___
## File di esempio

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

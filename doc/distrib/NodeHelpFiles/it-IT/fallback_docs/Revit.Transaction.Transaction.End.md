## In profondità
`Transaction.End` completa la transazione di Dynamo corrente e restituisce l'elemento specificato. In Revit, le transazioni rappresentano le modifiche apportate al documento di Revit. Quando si verifica una modifica, può essere visualizzata dal pulsante di annullamento attivato. Utilizzando `Transaction.End`, gli utenti possono aggiungere passaggi al grafico di Dynamo, creando un'azione di annullamento per ogni passaggio in cui viene utilizzato `Transaction.End`.

Nell'esempio seguente, viene posizionata un'istanza di famiglia nel documento di Revit. `Transaction.End` viene chiamato per completare il posizionamento prima di ruotare l'istanza di famiglia con `FamilyInstance.SetRotation`.

___
## File di esempio

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

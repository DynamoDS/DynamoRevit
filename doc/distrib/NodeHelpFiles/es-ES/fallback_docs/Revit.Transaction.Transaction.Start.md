## En detalle:
`Transaction.Start` inicia una transacción en el documento actual de Revit. Las transacciones se utilizan para agrupar los cambios realizados en el modelo de Revit a fin de que puedan confirmarse o revertirse de forma conjunta. `Transaction.Start` suele utilizarse junto con `Transaction.End` cuando un flujo de trabajo de Dynamo requiere un control explícito sobre cuándo comienzan y finalizan los cambios en el documento de Revit.

En el ejemplo siguiente, se crea un nivel y se utiliza como entrada para `Transaction.Start`. Esto inicia una transacción de Revit antes de que se realicen cambios en el modelo. Una vez completadas las operaciones necesarias en Revit, `Transaction.End` cierra la transacción y aplica los cambios al documento. El resultado es un objeto de transacción que puede transferirse a otros nodos relacionados con la transacción.
___
## Archivo de ejemplo

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

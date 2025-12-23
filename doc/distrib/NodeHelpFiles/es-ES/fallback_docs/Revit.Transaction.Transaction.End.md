## En detalle:
"Transaction.End" completa la transacción de Dynamo actual y devuelve el elemento especificado. En Revit, las transacciones representan los cambios realizados en el documento de Revit. Cuando se produce un cambio, se puede ver que el botón de deshacer se habilita. Con "Transaction.End", los usuarios pueden añadir pasos al gráfico de Dynamo, con lo que se crea una acción de deshacer para cada paso en el que se utilice "Transaction.End".

En el ejemplo siguiente, se coloca un ejemplar de familia en el documento de Revit. Se invoca a "Transaction.End" para completar la colocación antes de rotar el ejemplar de familia con "FamilyInstance.SetRotation".

___
## Archivo de ejemplo

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

## In Depth
This node starts a transaction in the current Revit document. Transactions are used to group changes made to the Revit model so they can be committed or rolled back together. This node is typically used with Transaction.End when a Dynamo workflow needs explicit control over when Revit document changes begin and finish. The output is the started transaction.

In this example, a level is created and used as the input to Transaction.Start. This begins a Revit transaction before model changes are made. After the required Revit operations are completed, Transaction.End closes the transaction and commits the changes to the document. The output of this node is a transaction object that can be passed to other transaction-related nodes.
___
## Example File

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
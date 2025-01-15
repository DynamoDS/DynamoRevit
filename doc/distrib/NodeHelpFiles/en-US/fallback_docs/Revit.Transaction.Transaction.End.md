## In Depth
`Transaction.End` completes the current Dynamo transaction and returns the specified element. In Revit, transactions represent changes made to the Revit document. When a change occurs, it can be seen by the enabled undo button. Using `Transaction.End`, users can add steps to the Dynamo graph, creating an undo action for each step where `Transaction.End` is used.

In the example below, a family instance is placed in the Revit document. `Transaction.End` is called to complete the placement before rotating the family instance with `FamilyInstance.SetRotation`.

___
## Example File

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
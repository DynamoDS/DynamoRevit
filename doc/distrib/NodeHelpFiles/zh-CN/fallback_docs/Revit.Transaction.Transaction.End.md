## 详细
“Transaction.End”完成当前 Dynamo 事务并返回指定的图元。在 Revit 中，事务表示对 Revit 文档所做的更改。发生更改时，可以通过启用的撤消按钮看到更改。使用“Transaction.End”，用户可以向 Dynamo 图形添加步骤，从而为使用“Transaction.End”的每个步骤创建撤消操作。

在下面的示例中，将族实例放置在 Revit 文档中。在使用“FamilyInstance.SetRotation”旋转族实例之前，会调用“Transaction.End”以完成放置。

___
## 示例文件

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

## 详细
“Transaction.Start”在当前 Revit 文档中启动事务。事务用于对对 Revit 模型所做的更改进行分组，以便可以一起提交或回滚这些更改。当 Dynamo 工作流需要精确控制 Revit 文档更改的开始和完成时间时，“Transaction.Start”通常与“Transaction.End”一起使用。

在下面的示例中，将创建一个关卡并将其用作“Transaction.Start”的输入。这将在模型更改之前开始 Revit 事务。在完成所需的 Revit 操作后，“Transaction.End”将关闭事务并将更改提交到文档。输出是一个可以传递给其他与事务相关的节点的交易对象。
___
## 示例文件

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

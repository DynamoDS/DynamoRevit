## 详细
Dynamo 中的 Revision.SetIssuedBy 节点用于设置或更新 Revit 中修订的“发布者”值。它通过记录修订的发布者来帮助自动控制修订，从而确保文档清晰一致，无需在 Revit 中进行手动编辑。

在此图形中，Select Revision 节点用于拾取所需的修订，字符串输入(例如 ABC)提供修订者的名称。然后，Revision.SetIssuedBy 节点将此值应用于选定修订，从而直接在 Revit 模型中更新“发布者”字段。

___
## 示例文件

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)

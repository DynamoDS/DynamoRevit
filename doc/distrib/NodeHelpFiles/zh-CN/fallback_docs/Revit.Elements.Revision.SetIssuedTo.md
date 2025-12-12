## 详细
Revision.SetIssuedTo 节点用于设置或更新 Revit 中修订的“发布对象”值。它有助于自动执行记录修订接收者的过程，从而确保项目文档的准确性和一致性，而无需在 Revit 中手动输入。

在此图形中，Select Revision 节点用于选择所需的修订，而字符串输入(例如 XYZ)定义收件人。然后，Revision.SetIssuedTo 节点将此值应用于选定修订，从而直接在 Revit 模型中更新“发布对象”字段。
___
## 示例文件

![Revision.SetIssuedTo](./Revit.Elements.Revision.SetIssuedTo_img.jpg)

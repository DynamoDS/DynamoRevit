## 详细
Dynamo 中的 Revision.Issued 节点用于检查 Revit 中的修订是否已标记为已发布。它会返回 true 或 false 值(布尔)，帮助团队快速验证修订的状态，而无需打开 Revit 修订设置。

在此图形中，Select Revision 节点用于从项目中选择修订。然后，Revision.Issued 节点检查选定的修订是否已发布，结果在 Watch 节点中显示为 true 或 false。这样，您可以直接通过 Dynamo 轻松确认修订的问题状态。

___
## 示例文件

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)

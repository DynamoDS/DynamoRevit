## 详细
Dynamo 中的 Revision.SetIssued 节点允许您控制 Revit 中的选定修订是将标记为已发布还是未发布。它需要一个修订图元和一个布尔输入(True/False)，使用户能够直接控制修订状态，而无需在 Revit 中手动编辑。
在此图形中，Select Revision 节点用于拾取特定修订(例如，“Seq. 1 – Schematic Design”)。布尔节点提供 True/False 值，该值随后会连接到 Revision.SetIssued 节点以自动更新修订的已发布状态。

___
## 示例文件

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

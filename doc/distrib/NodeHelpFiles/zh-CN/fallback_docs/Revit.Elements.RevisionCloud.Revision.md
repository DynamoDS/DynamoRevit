## 详细
此节点提取链接到 Revit 中特定云线批注的修订图元。它提供与该云线关联的修订数据，从而允许用户在其项目中以编程方式检查、跟踪或验证修订详细信息。

在此示例中，使用表示宽度和长度的数字滑块创建一个矩形，然后分解为曲线并反转以获得正确的方向。这些曲线以及选定视图(L1_SD)和选定修订(序列 2 - 不用于施工)用于通过 RevisionCloud.ByCurve 节点生成修订云线。生成的修订云线连接到 RevisionCloud.Revision 节点，该节点将检索并输出与该云线关联的修订。这可确保用户可以确认绑定到每个修订云线的修订。
___
## 示例文件

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)

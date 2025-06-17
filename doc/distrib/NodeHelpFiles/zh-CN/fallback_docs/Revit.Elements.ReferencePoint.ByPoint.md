## 详细
“ReferencePoint.ByPoint”在活动 Revit 族文档中的给定点位置处创建参照点图元。注意: 族文档必须是自适应构件或体量族。此节点与“ReferencePoint.ByCoordinates”不同，因为它将 Dynamo 点用于位置。这非常有用，因为最终用户可以使用直接操作来编辑 Dynamo 点，从而导致参照点也会更新。有关直接操作的详细信息，请访问此 [链接](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation)。

在下面的示例中，将在坐标 0,0,1 处创建一个新的参照点。
___
## 示例文件

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

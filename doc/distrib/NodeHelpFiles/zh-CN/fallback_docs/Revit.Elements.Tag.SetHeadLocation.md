## 详细
此节点获取标记并更改其头部位置。这使我们能够自动执行一致的放置行为，以便标记直接位于它们正在标记的图元之上。

在此示例中，在“Studio Live Work Core B”视图中选择了一扇门。将提取该门的位置，然后将其与 horizontal 和 addLeader 的布尔值一起用作 Tag.ByElementAndLocation 的原始输入。修改原始位置，以便标记位置不会使用 Tag.SetHeadLocation 节点直接覆盖在图元之上。

___
## 示例文件

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

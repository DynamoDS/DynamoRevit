## 详细
此节点标记给定视图、图元、位置、水平(如果没有，标记将基于图元定向)和 addLeader 作为输入的 Revit 图元。

在此示例中，在“Studio Live Work Core B”视图中选择了一扇门。将提取该门的位置，然后将其与 horizontal 和 addLeader 的布尔值一起用作 Tag.ByElementAndLocation 的原始输入。修改原始位置，以便标记位置不会使用 Tag.SetHeadLocation 节点直接覆盖在图元之上。

___
## 示例文件

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

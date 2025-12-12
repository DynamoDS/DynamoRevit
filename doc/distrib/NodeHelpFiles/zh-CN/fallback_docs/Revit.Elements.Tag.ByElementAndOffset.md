## 详细
此节点以给定的 view、element、offset、horizontalAligment、verticalAlignment、horizontalAlignment、horizontal (如果不是，标记将基于图元进行定向)和 addLeader 作为输入来标记 Revit 图元。

在此示例中，在“Studio Live Work Core B”视图中选择了一扇门，并将其用作 Tag.ByElementAndOffset 的输入。将提取该门的位置并用作向量起点。使用更改 x 和 y 点的滑块修改同一点，并将其用作矢量终点。此矢量与 horizontal 和 addLeader 输入中的 true 值一起用作偏移输入。horizontalAlignment 由“选择水平文字对齐”节点下拉值(左、中、右)和“选择垂直文字对齐”节点下拉值(底、中、顶)定义。

___
## 示例文件

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

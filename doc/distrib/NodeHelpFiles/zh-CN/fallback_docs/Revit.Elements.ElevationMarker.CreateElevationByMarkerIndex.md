## 详细
此节点通过指定标记索引，从现有 ElevationMarker 创建立面视图。Revit 中的每个立面标记最多可以容纳四个单独的立面视图 - 每个方向(北、南、东、西)一个立面视图。此节点允许您通过引用标记和所需的索引编号来生成其中一个方向立面。

在此示例中，将创建一个立面标记，并将其用作节点 ElevationMarker.CreateElevationByMarkerIndex 的输入 elevationMarker，以及视图和索引(0,1,2,3)。

___
## 示例文件

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

## 详细
Dynamo 中的 ReferencePlane.ByLine 节点通过使用定义的线作为其基准来创建 Revit 参照平面。这允许您在特定位置和方向上生成自定义参照平面。

在此示例中，使用带有可调整滑块的 Point.ByCoordinates 定义两个点。然后，在这两个点之间创建 Line.ByStartPointEndPoint，最后，ReferencePlane.ByLine 节点会沿该直线生成参照平面。
___
## 示例文件

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

## 详细
“ElementCurveReference.ByCurve”从 Dynamo 曲线创建 Revit 图元曲线参照。当另一个 Revit 节点需要可选或可参照的曲线而不仅仅是 Dynamo 几何图形时，曲线参照非常有用。这通常用于需要 Revit 参照的工作流，例如创建尺寸标注、路线、约束或其他需要参照模型几何图形的图元。

在下面的示例中，选择一条曲线并将其用作“ElementCurveReference.ByCurve”的输入。输出是“ElementCurveReference”，然后将其用作需要基于曲线的参照的下游节点的 Revit 曲线参照。
___
## 示例文件

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

## 详细
“Dimension.ByReferences”使用给定的参照沿选定视图中的输入线放置尺寸标注。输出为 Revit 尺寸标注图元。

在下面的示例中，创建多个点并使用“Line”节点连接以在 Revit 中生成模型曲线。然后，模型曲线将转换为 Revit 曲线参照。这些参照将合并到一个列表中，并用作“Dimension.ByReferences”的“references”输入。将创建其他线来定义尺寸标注的放置和方向。活动 Revit 视图也作为“view”输入提供。
___
## 示例文件

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

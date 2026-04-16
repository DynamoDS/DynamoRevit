## 详细
此节点将检索与给定草图平面图元关联的参照平面。这有助于识别或重复使用相同的参照平面来创建或修改几何图形。

在此示例中，定义了一个平面，然后将其连接到 SketchPlane.ByPlane 节点，这将生成相应的草图平面。此草图平面用作 SketchPlane.ElementPlaneReference 的输入，然后，在其中“out”可用于尺寸标注、对齐、约束或需要 Revit 参照的其他操作。

___
## 示例文件

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)

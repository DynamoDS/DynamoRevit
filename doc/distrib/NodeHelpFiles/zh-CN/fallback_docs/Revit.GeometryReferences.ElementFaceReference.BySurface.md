## 详细
“ElementFaceReference.BySurface”从选定或生成的曲面创建 Revit 面参照。它会将与 Revit 几何图形关联的 Dynamo 曲面转换为“ElementFaceReference”。然后，需要 Revit 面参照而不仅仅是曲面几何图形的其他节点可以使用此参照。

在下面的示例中，将创建一面墙，并将该墙中的曲面用作“ElementFaceReference.BySurface”的输入。输出是一个“ElementFaceReference”，需要参照 Revit 图元面的节点可以使用该节点。
___
## 示例文件

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

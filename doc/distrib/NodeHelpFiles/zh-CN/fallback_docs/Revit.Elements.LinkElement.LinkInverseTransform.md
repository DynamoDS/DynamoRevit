## 详细
此节点检索给定 Revit 链接图元的逆变换矩阵。在 Revit 中，链接模型可以通过变换(平移、旋转、缩放)进行定位。此节点可以获取该变换的逆变换，从而有效地将链接模型空间的坐标转换回主体 Revit 模型的坐标系。

在此示例中，将选择级别 L3 的所有 Revit 链接图元并输入到 LinkElement.LinkInverseTransform。输出为主体 Revit 模型的坐标系。
___
## 示例文件

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

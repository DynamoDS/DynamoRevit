## 详细
此节点检索应用于主体模型内 Revit 链接图元的变换矩阵。
换句话说，它返回将链接图元的坐标系映射到主体 Revit 项目坐标系的位置、旋转和缩放变换。
这在需要对齐、分析或操纵链接模型之间的几何图形时非常有用。

在此示例中，将选择级别 L3 的所有 Revit 链接图元并输入到 LinkElement.LinkTransform。输出是链接图元的位置、旋转和缩放变换。
___
## 示例文件

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

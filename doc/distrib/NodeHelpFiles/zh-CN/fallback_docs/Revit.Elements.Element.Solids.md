## 详细
“Element.Solids”返回给定图元的实体几何图形。实体以嵌套列表的形式返回，因为任何给定图元中都可以有多个实体。如果需要表示图元的单个实体，则可以在列表上使用“Solid.ByUnion”。

在下面的示例中，将从选定视图收集所有墙。然后，将删除作为内建族创建的墙，并返回剩余墙的实体。

___
## 示例文件

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

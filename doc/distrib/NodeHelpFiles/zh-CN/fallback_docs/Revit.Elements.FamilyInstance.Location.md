## 详细
“FamilyInstance.Location”返回给定族实例的 Dynamo 点。如果没有位置，则返回空值。“FamilyInstance.Location”适用于基于点的图元，不会返回 Revit 中基于曲线的图元的位置，例如墙或梁图元。

在下面的示例中，将收集当前文档的当前视图中的所有门族实例。然后，使用“FamilyInstance.Location”返回门的位置。

在此示例中，幕墙门返回空值。幕墙嵌板的位置可通过幕墙嵌板节点获得。
___
## 示例文件

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

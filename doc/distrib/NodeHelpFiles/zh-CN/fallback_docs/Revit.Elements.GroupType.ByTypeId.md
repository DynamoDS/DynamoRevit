## 详细
“GroupType.ByTypeId”主要用于必须以编程方式指定 Revit 参数组的参数创建和管理工作流。输入是一个字符串，表示 Revit 参数组类型 ID。输出是“GroupType”对象，可以在需要参数分组分类的节点的下游使用该对象。

在下面的示例中，创建了表示 Revit 参数组类型 ID 的多个字符串，并将其组合到一个列表中。然后，该列表将用作“GroupType.ByTypeId”的输入。输出是表示相应参数组的 Revit“GroupType”对象的列表。
___
## 示例文件

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)

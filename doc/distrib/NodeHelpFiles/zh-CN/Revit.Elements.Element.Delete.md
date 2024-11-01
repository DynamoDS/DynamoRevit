## 详细
“Element.Delete”的操作方式与 Revit 界面中的删除选项相同。它将删除输入图元以及依赖它的任何图元。

例如，删除包含门的墙也将删除门。输出由因此删除的图元的图元 ID 的嵌套列表组成。注意: 此节点最适用于 Dynamo 中的手动执行模式。

在下面的示例中，所有“帮助按钮”族实例都从当前文档(文件)中删除。
___
## 示例文件

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

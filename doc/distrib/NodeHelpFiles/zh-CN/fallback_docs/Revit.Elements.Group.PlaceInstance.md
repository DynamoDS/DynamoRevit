## 详细
放置以位置(点)和 groupType 作为输入的 Revit 组。groupType 的 Object.Type 输出会读取 Revit.Elements.ElementType

 在下面的示例中，所有模型组都收集自活动的 Revit 文档。组的类型使用 Group.GroupType 返回，减少到项目列表中的第一个组，并用作 GroupType 的输入。位置输入放置在 100,100 处。
___
## 示例文件

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)

## 详细
“Element.SetParameterByName”将参数图元(按名称查找)设置为给定值。值包括：双精度、整数、布尔、图元 ID、图元和字符串。在 Revit 中，参数可以共享相同的名称。因此，“Element.SetParameterByName”将按字母顺序在找到的第一个参数上设置值按唯一 ID。

在下面的示例中，正在为模型中位于房间内的所有家具项目设置注释参数。注释参数的值是获得的房间名称。
___
## 示例文件

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

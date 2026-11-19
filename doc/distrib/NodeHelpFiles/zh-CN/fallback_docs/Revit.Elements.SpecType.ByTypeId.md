## 详细
“SpecType.ByTypeId”将 Revit 等级库类型 ID 字符串转换为“SpecType”对象。等级库类型定义参数在 Revit 中表示的数据类型，例如长度、面积、角度、材质、力、文字、整数等。在较新版本的 Revit 中创建共享参数、项目参数或参数定义时，通常使用此方法，其中 Autodesk 已将较旧的“ParameterType”系统替换为 Forge TypeIds。

在下面的示例中，表示 Revit 等级库类型 ID 的字符串提供给“SpecType.ByTypeId”。输出是一个“SpecType”对象，可以在创建新参数定义时在下游使用，以便 Revit 了解参数应存储的数据类型。
___
## 示例文件

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

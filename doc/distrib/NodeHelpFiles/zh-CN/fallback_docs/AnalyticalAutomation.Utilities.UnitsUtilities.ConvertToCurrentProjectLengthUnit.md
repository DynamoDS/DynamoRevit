## 详细
此节点采用数字长度值和单位类型标识符，将输入值转换为活动 Revit 项目的长度单位。输出是一个双精度值，表示转换后的结果。

在此示例中，数字滑块提供长度值，选择单位(例如“米”)以获取其 Unit.TypeId 字符串。两者都连接到 UnitsUtilities.ConvertToCurrentProjectLengthUnit 节点，该节点将基于项目的单位设置返回转换后的长度值。
___
## 示例文件

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

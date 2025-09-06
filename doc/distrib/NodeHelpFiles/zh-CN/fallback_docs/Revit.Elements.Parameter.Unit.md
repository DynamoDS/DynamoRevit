## 详细

返回参数的单位类型。

在下面的示例中，我们将提取所有图元参数，并将它们用作输入。目标是显示每个参数的单位类型。
例如，如果 “Element.Parameters” 显示“Base Diameter : 1’ – 3 ¼”“，”Parameter.Unit“的输出将为”Unit (Name = Feet or Meters)”。
如果参数没有单位(例如 Element.Parameters = Apply Cut : No)，则 Parameter.Unit 将返回空值。
由于 Dynamo 本身是无单位的，因此确定传入的单位类型以执行精确计算至关重要。此节点有助于 Dynamo 识别和处理该单位数据。

___
## 示例文件

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

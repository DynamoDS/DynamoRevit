## 详细
此节点计算给定表面，以确定它是否可用于定义分析面板。有效表面通常是平面的、连续的，并且适合在 Revit 的分析模型环境中转换为分析表示。

在此示例中，将收集项目中板图元的面，并将顶面作为输入提供给节点。该节点将返回一个布尔结果，指示选定表面是否满足创建分析面板的要求，还会返回一条可选消息，描述在验证过程中遇到的任何问题。
___
## 示例文件

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

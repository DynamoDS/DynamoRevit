## 详细
此节点通过检查指定索引处的力值是否在定义的范围内来过滤 ConnectionNode 列表。力数据来自结构分析结果或 Revit 分析模型，并按选定的结果类型(例如，Fx、Fy、Fz、Mx、My、Mz)过滤。

在此示例中，使用所选的分析结果和荷载工况，基于 Fz 力分量选择和评估一组柱图元。只有 Fz 值在指定力范围内的那些图元才会返回为接受的连接。
___
## 示例文件

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

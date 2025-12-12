## 详细
此节点根据输入图元在模型中的垂直位置过滤输入图元。它允许您选择是计算图元的顶部还是底部 Z 坐标(高度)以进行比较或进行逻辑过滤。它通常用作过滤系统的一部分，通常与 ConnectionNode 结合使用，以隔离高于或低于特定标高的图元。它在涉及空间分析的工作流中非常有用，例如按标高或分区分离建筑图元。

在本示例中，我们按“顶部”Z 坐标位置(高度)过滤所有所选结构数据项目。这可用于进一步确定位置是否低于标高、楼板或天花板。
___
## 示例文件

![Filter.ByVerticalPosition](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByVerticalPosition_img.jpg)

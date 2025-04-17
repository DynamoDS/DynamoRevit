## 详细
给定支持托管图元(例如墙)的图元，“Element.GetHostedElements”返回依赖给定图元的图元。默认情况下，返回托管到该图元族实例。“Element.GetHostedElements”提供包含其他托管图元类型的选项。

它们包括:
- 开口
- 在相连主体中托管的图元
- 内嵌墙(即幕墙)
- 共享嵌入式嵌件

在下面的示例中，将从 L2 收集所有墙图元。然后，使用“Element.GetHostedElements”检查墙图元中是否有主体图元。然后，此列表将用于创建 2 个列表。有门的墙和没有门的墙。
___
## 示例文件

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

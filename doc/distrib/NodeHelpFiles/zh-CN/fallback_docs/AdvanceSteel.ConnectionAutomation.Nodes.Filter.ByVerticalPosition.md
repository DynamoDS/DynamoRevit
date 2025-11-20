## 详细
This node filters input elements based on their vertical placement within the model. It allows you to choose whether to evaluate an element’s Top or Bottom Z-coordinate (height) for comparison or logical filtering. Its typically used as part of a filtering system, often in conjunction with a ConnectionNode, to isolate elements above or below certain elevations. It is useful in workflows that involve spatial analysis, such as separating building elements by level or zone.

In this example, we are filtering all the selected structure data items by their "Top" z-coordinate position (height). This can be used to further determine if the position is lower than a level, floor, or ceiling.
___
## 示例文件

![Filter.ByVerticalPosition](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByVerticalPosition_img.jpg)

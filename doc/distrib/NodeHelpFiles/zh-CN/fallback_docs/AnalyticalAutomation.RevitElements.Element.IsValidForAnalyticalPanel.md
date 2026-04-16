## 详细
此节点计算选定图元以确定它是否适合生成分析面板。可选的 checkOpenings 输入指定是否应将元素内的洞口包含在有效性检查中。当设置为 true 时，洞口会被视为评估的一部分。

在此示例中，图元是使用 Element.ById 节点由其图元 ID 定义的，并提供给 Element.IsValidForAnalyticalPanel。输出包括一个布尔值，指示该图元是否有效，以及一条异常消息，描述阻止其使用的任何问题。
___
## 示例文件

![Element.IsValidForAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.IsValidForAnalyticalPanel_img.jpg)

## 详细
从 ScheduleView 的给定输入中删除所有定义的过滤器。

在此示例中，我们最初定义了一个计划视图，该视图直接路由到 ScheduleView.ScheduleFilters 以显式显示其预先存在的视图过滤器。为了突出显示转换，同一视图会经过 10 毫秒的短暂停顿。在此短暂延迟后，视图将转到 ScheduleView.ClearAllFilters 节点，该节点会删除所有应用的过滤器。最后，现在修改后的视图将引导回另一个 ScheduleView.ScheduleFilters 节点，明确地表明其过滤器列表已成为空列表，从而确认已成功清除所有原始过滤器。
___
## 示例文件

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

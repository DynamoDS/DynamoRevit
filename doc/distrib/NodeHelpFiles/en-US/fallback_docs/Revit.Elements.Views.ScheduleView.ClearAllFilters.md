## In Depth
Removes all defined filters from a given input of a ScheduleView.

In this example, we initially defined a schedule view that is routed directly to ScheduleView.ScheduleFilters to explicitly show its pre-existing view filters. To highlight the transformation, this same view then passes through a brief 10-millisecond pause. After this short delay, the view proceeds to the ScheduleView.ClearAllFilters node, which removes all applied filters. Finally, the now-modified view is channeled back into another ScheduleView.ScheduleFilters node, unequivocally demonstrating that its filter list has become an empty list, thereby confirming the successful clearing of all original filters.
___
## Example File

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)
## In profondità
`Sheet.Schedules` returns the schedule instances placed on the given sheet. Note: `Sheet.Schedules` will return revision schedules that exist within the titleblock family for the given sheet.

In the example below, the schedule instances are returned for the selected sheet. The revision schedules are then filtered out with `List.FilterByBoolMask`. Additionally, the owner view for the schedule instance is retrieved with `ScheduleOnSheet.ScheduleView`.
___
## File di esempio

![Sheet.Schedules](./Revit.Elements.Views.Sheet.Schedules_img.jpg)

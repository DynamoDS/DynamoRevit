## Description approfondie
This node creates a schedule filter by identifying a field, a filter type and a value.  This filter can then be used in a schedule.

In this example, a view (WV_Wall Scheudle) is selected, the schedule fields are exposed, “Type” is selected and used as the input to field in node ScheduleFilter.ByFieldTypeAndValue along with the filter type “BeginsWIth” and a value “Exterior”.  This filter type is then used to change what is displayed in schedule view WV_Wall Scheudle.  This filter only shows walls where the type begins with “Exterior.”

___
## Exemple de fichier

![ScheduleFilter.ByFieldTypeAndValue](./Revit.Schedules.ScheduleFilter.ByFieldTypeAndValue_img.jpg)

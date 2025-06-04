## In Depth
`Level.ProjectElevation` returns the elevation for the given level in project units. `Level.ProjectElevation` reports the value from project origin. If the elevation from ground level is required, this value can be obtained with `Level.Elevation`.

In the example below, all levels are collected in the current Revit document. The levels' project elevation value are returned. Additionally, the levels are sorted by height with "List.SortByKey".
___
## Example File

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)

## In Depth
`Level.Elevation` returns the elevation for the given level in project units. `Level.Elevation` reports the value from ground level. If the project's elevation differs, this can be returned with `Level.ProjectElevation`.

In the example below, all levels are collected in the current Revit document. The levels' elevation value are returned. Additionally, the levels are sorted by height with "List.SortByKey".
___
## Example File

![Level.Elevation](./Revit.Elements.Level.Elevation_img.jpg)

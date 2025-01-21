## 深入資訊
`Level.Elevation` 會傳回以專案單位表示的給定樓層高程。`Level.Elevation` 會顯示從地面樓層開始的值。如果專案的高程不同，可以使用 `Level.ProjectElevation` 傳回此值。

以下範例收集目前 Revit 文件中的所有樓層，然後傳回樓層的高程值。此外，使用 `List.SortByKey` 依高度排序樓層。
___
## 範例檔案

![Level.Elevation](./Revit.Elements.Level.Elevation_img.jpg)

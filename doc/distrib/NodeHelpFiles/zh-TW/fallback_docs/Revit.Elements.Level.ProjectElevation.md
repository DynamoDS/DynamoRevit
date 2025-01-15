## 深入資訊
`Level.ProjectElevation` 會傳回以專案單位表示的給定樓層高程。`Level.ProjectElevation` 會顯示專案原點的值。如果需要從地面樓層開始的高程，可以使用 `Level.Elevation` 取得此值。

以下範例收集目前 Revit 文件中的所有樓層，然後傳回樓層的專案高程值。此外，使用 `List.SortByKey` 依高度排序樓層。
___
## 範例檔案

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)

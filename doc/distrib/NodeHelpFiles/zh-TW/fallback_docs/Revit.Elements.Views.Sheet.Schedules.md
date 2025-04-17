## 深入資訊
`Sheet.Schedules` 會傳回放置在給定圖紙上的明細表實體。注意事項: `Sheet.Schedules` 會傳回在給定圖紙的圖框族群內的修訂明細表。

以下範例會傳回所選圖紙的明細表實體，然後使用 `List.FilterByBoolMask` 篩選出修訂明細表。此外，還使用 `ScheduleOnSheet.ScheduleView` 擷取明細表實體的擁有者視圖。
___
## 範例檔案

![Sheet.Schedules](./Revit.Elements.Views.Sheet.Schedules_img.jpg)

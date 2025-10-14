## 深入資訊
移除 ScheduleView 給定輸入的所有已定義篩選。

在此範例中，我們最初定義一個明細表視圖直接路由到 ScheduleView.ScheduleFilters，以明確顯示其預先存在的視圖篩選。為了亮顯轉換，此相同視圖會經過短暫的 10 毫秒暫停。在此短暫的延遲之後，視圖會進入 ScheduleView.ClearAllFilters 節點，這會移除所有套用的篩選。最後，將現在修改後的視圖引導回另一個 ScheduleView.ScheduleFilters 節點，明確表示其篩選清單已變成空清單，進而確認已成功清除所有原始篩選。
___
## 範例檔案

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

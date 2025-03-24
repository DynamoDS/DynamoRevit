## 深入資訊
`ScheduleFilter.FilterType` 會傳回用於輸入篩選的方法。
可能的篩選類型包括:

- Equal - 欄位值等於指定值。
- NotEqual - 欄位值不等於指定值。
- GreaterThan - 欄位值大於指定值。
- GreaterThanOrEqual - 欄位值大於或等於指定值。
- LessThan - 欄位值小於指定值。
- LessThanOrEqual - 欄位值小於或等於指定值。
- Contains - 用於字串欄位，欄位值包含指定字串。
- NotContains - 用於字串欄位，欄位值不包含指定字串。
- BeginsWith - 用於字串欄位，欄位值以指定字串為開頭。
- NotBeginsWith - 用於字串欄位，欄位值不以指定字串為開頭。
- EndsWith - 用於字串欄位，欄位值以指定字串為結尾。
- NotEndsWith - 用於字串欄位，欄位值不以指定字串為結尾。
- IsAssociatedWithGlobalParameter - 此欄位與相容類型的指定整體參數相關聯
- IsNotAssociatedWithGlobalParameter - 此欄位與相容類型的指定整體參數無關聯

以下範例收集目前 Revit 檔案中的第一個明細表，然後檢查明細表視圖是否有篩選，套用的唯一篩選是「字串最後一個字母不是」篩選類型。
___
## 範例檔案

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

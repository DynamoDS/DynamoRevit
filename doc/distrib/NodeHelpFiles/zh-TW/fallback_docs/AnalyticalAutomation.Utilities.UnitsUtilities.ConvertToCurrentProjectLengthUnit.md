## 深入資訊
此節點會使用數值長度值和單位類型識別碼，將輸入值轉換為作用中 Revit 專案的長度單位。輸出是一個表示轉換結果的倍精度值。

在此範例中，數字滑棒提供長度值，選取單位 (例如，公尺) 取得其 Unit.TypeId 字串。兩者都連接至 UnitsUtilities.ConvertToCurrentProjectLengthUnit 節點，該節點會根據專案的單位設定傳回轉換後的長度值。
___
## 範例檔案

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

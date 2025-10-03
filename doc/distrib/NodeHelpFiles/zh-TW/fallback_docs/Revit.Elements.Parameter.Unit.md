## 深入資訊

傳回參數的單位類型。

在以下範例中，我們擷取所有元素參數，將它們當作輸入。目標是顯示每個參數的單位類型。
例如，如果 Element.Parameters 顯示「Base Diameter : 1’ – 3 ¼”」，則 Parameter.Unit 的輸出將為「單位 (名稱 = Feet 或 Meters)」。
如果參數沒有單位 (例如 Element.Parameters = 套用切割 : 否)，則 Parameter.Unit 將傳回空值。
由於 Dynamo 本身沒有單位，因此必須識別傳入的單位類型才能執行準確的計算。此節點可協助 Dynamo 辨識和處理該單位資料。

___
## 範例檔案

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

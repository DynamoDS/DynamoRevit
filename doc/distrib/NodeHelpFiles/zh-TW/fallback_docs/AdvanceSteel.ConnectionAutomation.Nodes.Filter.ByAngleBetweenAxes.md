## 深入資訊
此節點會傳回依最小-最大度數範圍和選取的軸篩選的接頭節點清單。

在此範例中，選取一根 W 造型柱和一個斜角 HSS 斜撐，輸出包含一個具有「accepted」和「rejected」性質的字典清單。accepted 值為接頭節點，且比較兩個元素的 x 軸時在 0-60 度的範圍內。如果最大值是 45 度，接頭節點將被拒絕。`indexFirst` 和 `indexSecond` 參數設定為「使用層級」以便與輸入資料結構一致。
___
## 範例檔案

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

## 深入資訊
此節點會檢查指定索引處的力值是否落在定義的範圍內，來篩選 ConnectionNodes 的清單。力資料來自於結構分析結果或 Revit 分析模型，並依選取的結果類型 (例如 Fx、Fy、Fz、Mx、My、Mz) 篩選。

在此範例中，選取一組柱元素，並使用選擇的分析結果和負載情況，根據 Fz 力分量進行演算。只傳回其 Fz 值在指定力範圍內的元素作為可接受的接頭。
___
## 範例檔案

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

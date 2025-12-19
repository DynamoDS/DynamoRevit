## 深入資訊
此節點會擷取與給定 Revit CurveElement 相關聯的曲線參考，例如模型曲線或細部線，然後可以使用該參考作為其他需要幾何圖形參考 (例如尺寸標註、對齊或建立分割路徑) 的節點的輸入。

在此範例中，使用起點與終點建立模型曲線，然後輸入到 CurveElement.ElementCurveReference 節點。輸出是可用於下游操作的曲線元素幾何參考。
___
## 範例檔案

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

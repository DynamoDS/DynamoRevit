## 深入資訊
`Dimension.ByReferences` 使用給定的參考，沿著所選視圖的輸入線放置尺寸標註。輸出是 Revit 尺寸標註元素。

以下範例建立幾個點並使用 Line 節點連接，在 Revit 中產生模型曲線。然後將模型曲線轉換為 Revit 曲線參考。這些參考會合併為清單，並作為 `Dimension.ByReferences` 的 `references` 輸入。建立其他線來定義尺寸標註的位置和方向。同時提供作用中的 Revit 視圖作為 `view` 輸入。
___
## 範例檔案

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

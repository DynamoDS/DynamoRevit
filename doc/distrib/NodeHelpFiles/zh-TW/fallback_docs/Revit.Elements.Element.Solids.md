## 深入資訊
`Element.Solids` 會傳回給定元素的實體幾何圖形。以巢狀清單傳回實體，因為任何給定元素當中都可以有多個實體。如果需要表示元素的單個實體，可以對清單使用 `Solid.ByUnion`。

以下範例收集所選視圖中的所有牆，然後移除以現地族群建立的牆，並傳回其餘牆的實體。

___
## 範例檔案

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

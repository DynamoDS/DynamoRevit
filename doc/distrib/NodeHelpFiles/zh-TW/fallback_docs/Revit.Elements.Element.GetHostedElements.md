## 深入資訊
給定支撐元素主體 _(例如牆)_ 的元素，`Element.GetHostedElements` 會傳回依賴給定元素的元素。預設會傳回以該元素為主體的族群實體。`Element.GetHostedElements` 提供包括其他主體元素類型的選項。

包括:
- 開口
- 以接合的主體為主體的元素
- 嵌入牆 _(即帷幕牆)_
- 共用的嵌入式嵌入件

以下範例 收集 L2 的所有牆元素，然後使用 `Element.GetHostedElements` 檢查牆元素是否有主體元素，接著使用此清單建立兩個清單。有門的牆和沒有門的牆。
___
## 範例檔案

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

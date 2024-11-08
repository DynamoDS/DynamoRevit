## 深入資訊
`Element.Delete` 的運作方式與 Revit 介面中的刪除選項相同，會刪除輸入元素及任何依賴它的元素。

例如，刪除當中有門的牆，也會刪除門。輸出結果包含刪除之元素的元素 ID 巢狀清單。注意事項: 此節點最適合在 Dynamo 中的手動執行模式下使用。

以下範例刪除目前文件 (檔案) 中的所有「說明按鈕」族群實體。
___
## 範例檔案

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

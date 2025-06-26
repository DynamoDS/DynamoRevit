## 深入資訊
`Transaction.End` 會完成目前的 Dynamo 交易，並傳回指定的元素。在 Revit 中，交易記錄表示對 Revit 文件所做的變更。發生變更時，透過啟用的退回按鈕可以看見變更。使用者可以利用 `Transaction.End` 在 Dynamo 圖表中增加步驟，為使用 `Transaction.End` 的每個步驟建立一個退回動作。

以下範例在 Revit 文件中放置族群實體。先呼叫 `Transaction.End` 完成放置，再使用 `FamilyInstance.SetRotation` 旋轉族群實體。

___
## 範例檔案

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

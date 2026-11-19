## 深入資訊
`Transaction.Start` 會在目前 Revit 文件中啟動交易。交易的用途是為了將對 Revit 模型所做的變更分組，方便將這些變更一起提交或撤回。如果 Dynamo 工作流程需要明確控制 Revit 文件變更開始和完成的時間，通常會將 `Transaction.Start` 搭配 `Transaction.End` 使用。

以下範例建立一個樓層並作為 `Transaction.Start` 的輸入。如此便在模型變更之前開始 Revit 交易。所需的 Revit 作業完成後，`Transaction.End` 會關閉交易並將變更提交到文件。輸出是一個交易物件，可以傳遞給其他與交易相關的節點。
___
## 範例檔案

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

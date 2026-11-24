## 深入資訊
`GroupType.ByTypeId` 主要用於必須以程式設計方式指定 Revit 參數群組的參數建立和管理工作流程。輸入是表示 Revit 參數群組類型 ID 的字串。輸出是一個 `GroupType` 物件，可在需要參數分組分類的下游節點中使用。

以下範例建立幾個代表 Revit 參數群組類型 ID 的字串，並合併成一個清單。然後使用該清單作為 `GroupType.ByTypeId` 的輸入。輸出是代表對應參數群組的 Revit `GroupType` 物件清單。
___
## 範例檔案

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)

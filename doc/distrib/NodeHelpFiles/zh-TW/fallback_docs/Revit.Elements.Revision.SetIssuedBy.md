## 深入資訊
使用 Dynamo 中的 Revision.SetIssuedBy 節點設定或更新 Revit 中某個修訂的「發佈者」值，透過記錄修訂的發佈者來協助自動控制修訂，可確保文件清楚一致，無須在 Revit 中手動編輯。

在此圖表中，使用 Select Revision 節點挑選所需的修訂，字串輸入 (例如 ABC) 提供發佈者的名稱，然後 Revision.SetIssuedBy 節點會將此值套用到選取的修訂，直接更新 Revit 模型中的「發佈者」欄位。

___
## 範例檔案

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)

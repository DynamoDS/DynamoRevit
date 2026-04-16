## 深入資訊
Dynamo 中的 Revision.SetIssued 節點可讓您控制 Revit 中選取的修訂是否標記為已發佈。節點會使用修訂元素和布林輸入 (True/False)，讓使用者可以直接控制修訂狀態，無須在 Revit 中手動編輯。
在此圖表中，使用 Select Revision 節挑選特定修訂 (例如，「Seq. 1 - Schematic Design」)。Boolean 節點提供 True/False 值，然後連接到 Revision.SetIssued 節點自動更新修訂的發佈狀態。

___
## 範例檔案

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

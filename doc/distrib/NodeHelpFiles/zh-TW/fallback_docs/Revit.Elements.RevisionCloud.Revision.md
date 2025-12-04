## 深入資訊
此節點會擷取連結到 Revit 中特定修訂雲形的修訂元素，提供與該雲形相關聯的修訂資料，允許使用者在專案中以程式設計方式檢查、追蹤或驗證修訂詳細資料。

在此範例中，使用數字滑棒表示寬度和長度來建立一個矩形，然後將其分解成曲線並根據正確方位反轉。使用這些曲線以及選擇的視圖 (L1_SD) 和選取的修訂 (Seq. 2 – Not For Construction) 透過 RevisionCloud.ByCurve 節點產生修訂雲形。將產生的修訂雲形連接到 RevisionCloud.Revision 節點，擷取並輸出與該雲形相關聯的修訂。這可確保使用者能夠確認每個修訂雲形連結到的修訂。
___
## 範例檔案

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)

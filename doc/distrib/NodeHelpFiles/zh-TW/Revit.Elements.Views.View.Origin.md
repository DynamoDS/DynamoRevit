## 深入資訊
Revit 中的每個視圖都有一個原點。`View.Origin` 會傳回此值作為 Dynamo 的一點。根據 Revit API 文件，「平面視圖的原點沒有意義」。因此請記住，`View.Origin` 提供的值由終端使用者決定。

以下範例傳回作用中視圖的原點和選取的 3D 視圖。
___
## 範例檔案

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)

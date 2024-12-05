## 深入資訊
`FamilyInstance.Location` 會傳回給定族群實體的 Dynamo 點。如果沒有位置，則傳回空值。`FamilyInstance.Location` 適用於以點為基礎的元素，不會為 Revit 中以曲線為基礎的元素 _例如牆或樑元素_ 傳回位置。

以下範例收集目前文件之目前視圖中的所有門族群實體，然後使用 `FamilyInstance.Location` 傳回門的位置。

在本範例中，帷幕牆門傳回空值。透過帷幕板節點可取得帷幕板位置。
___
## 範例檔案

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

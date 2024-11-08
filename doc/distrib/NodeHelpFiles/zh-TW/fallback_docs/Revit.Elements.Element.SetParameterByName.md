## 深入資訊
`Element.SetParameterByName` 會將透過名稱找到的參數元素設定為給定值。值包括: double、integer、boolean、ElementId、Element 和 string。在 Revit 中，參數可以共用相同的名稱。因此，`Element.SetParameterByName` 將對透過 UniqueId 找到並依字母順序排序的第一個參數設定值。

以下範例為模型中位於房間內的所有家具項目設定備註參數。備註參數的值是取得的房間名稱。
___
## 範例檔案

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

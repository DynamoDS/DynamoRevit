## 深入資訊
`Subelement.GetAllParameters` 會擷取子元素的內部 Revit 參數 ID，而非參數顯示名稱。

以下範例選取目前視圖中所有的元素，篩選後只顯示包含子元素的元素。然後使用子元素作為 `Subelement.GetAllParameters` 的輸入。輸出是與每個子元素關聯的參數 ID 清單。最後一個節點顯示用來擷取參數值的輸出。

___
## 範例檔案

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)

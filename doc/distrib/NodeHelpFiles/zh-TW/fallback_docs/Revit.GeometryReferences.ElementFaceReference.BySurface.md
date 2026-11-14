## 深入資訊
`ElementFaceReference.BySurface` 會從選取或產生的曲面建立 Revit 面參考。它會將與 Revit 幾何圖形關聯的 Dynamo 曲面轉換為 `ElementFaceReference`。之後其他需要 Revit 面參考 (而不只是曲面幾何圖形) 的節點就可以使用此參考。

以下範例建立一面牆，然後使用該面牆的曲面作為 `ElementFaceReference.BySurface` 的輸入。需要參考 Revit 元素面的節點可以使用 `ElementFaceReference` 輸出。
___
## 範例檔案

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

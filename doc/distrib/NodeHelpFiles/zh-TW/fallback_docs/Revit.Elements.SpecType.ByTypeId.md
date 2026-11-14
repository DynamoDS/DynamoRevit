## 深入資訊
`SpecType.ByTypeId` 會將 Revit Spec TypeId 字串轉換為 `SpecType` 物件。「規格類型」定義參數在 Revit 中表示的資料種類，例如長度、面積、角度、材料、力、文字、整數等等。在較新版本的 Revit 中 (Autodesk 以 Forge TypeId 取代舊版 `ParameterType` 系統) 建立共用參數、專案參數或參數定義時，通常會使用此選項。

以下範例將表示 Revit Spec TypeId 的字串提供給 `SpecType.ByTypeId`。輸出是 `SpecType` 物件，可在建立新參數定義時在下游使用，讓 Revit 瞭解參數應儲存的資料類型。
___
## 範例檔案

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

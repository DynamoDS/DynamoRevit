## 深入資訊
與 `Revit.Elements.FamilyType.ByFamilyAndName` 類似，`Revit.Elements.FamilyType.ByFamilyNameAndTypeName` 會傳回目前文件中的族群類型定義 (如果有)。這與 `Revit.Elements.FamilyType.ByFamilyAndName` 類似。但是，此節點不使用族群定義，而是依賴兩個值的字串輸入。如果目前文件中沒有族群類型，則傳回空值。

在以下範例中，傳回「門-通道-單-嵌平」族群中的「36" x 84"」門族群類型。
___
## 範例檔案

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)

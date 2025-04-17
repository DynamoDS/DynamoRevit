## 深入資訊
`FamilyType.ByName` 會嘗試從目前文件中擷取給定名稱的給定族群類型。如果目前文件沒有族群類型，則傳回空值。

注意事項: `FamilyType.ByName` 會依父系族群的建立順序 (依元素 ID) 搜尋族群類型定義。如果多個父系族群有名稱相同的類型定義，則傳回找到的第一個父系族群。若要更簡潔地查詢族群類型，請使用 `FamilyType.ByFamilyAndName` 或 `FamilyType.ByFamilyNameAndTypeName`

在以下範例中，傳回「門-通道-單-嵌平」族群中的「36" x 84"」門族群類型。
___
## 範例檔案

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)

## 深入資訊
`Space.IsPointInsideSpace` 會檢查給定點是否在給定空間內。將標記值指定給 Revit 內的元素時會很有用。

以下範例收集目前 Revit 文件之給定視圖中的所有空調風口，然後使用 `Space.IsPointInsideSpace` 將風口的點位置與給定視圖中的空間進行比較。使用清單管理，建立子清單以篩選掉空間內出現的空調風口。如需使用 List@Level 的更多資訊，請參閱此 [文章](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level)。
___
## 範例檔案

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

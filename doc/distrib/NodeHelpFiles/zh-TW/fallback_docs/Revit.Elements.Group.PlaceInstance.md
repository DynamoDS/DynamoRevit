## 深入資訊
以 location (點) 和 groupType 作為輸入，放置 Revit 群組。groupType 的 Object.Type 輸出會讀取 Revit.Elements.ElementType

 以下範例收集作用中 Revit 文件中的所有模型群組。使用 Group.GroupType 傳回群組的類型，精簡為專案清單中的第一個群組，並用作 GroupType 的輸入。location 輸入放在 100,100。
___
## 範例檔案

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)

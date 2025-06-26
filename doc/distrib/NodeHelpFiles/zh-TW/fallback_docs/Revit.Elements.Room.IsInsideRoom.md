## 深入資訊
`Room.IsInsideRoom` 會傳回布林值，指出是否在給定房間中找到給定點。

以下範例收集目前 Revit 文件中的所有家具以及所有房間，然後將家具的位置點傳入 `Room.IsInsideRoom`，確認給定點出現在哪個房間 (如果有)。最後，對家具進行篩選，找出具有房間位置的元素，然後將這些值合併成清單。
___
## 範例檔案

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

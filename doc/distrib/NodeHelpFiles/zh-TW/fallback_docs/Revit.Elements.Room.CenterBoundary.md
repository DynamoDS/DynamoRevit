## 深入資訊
`Room.CenterBoundary` 會傳回表示給定房間中心線邊界的巢狀清單。在傳回的清單中，第一個子清單表示最外側的曲線，後續的清單表示房間內的迴圈。中心邊界位於 Revit 房間內所有層的牆中心線上。如需牆定位線的更多資訊，請參閱此說明 [文章](https://help.autodesk.com/view/RVT/2024/CHT/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)。

如果給定未設邊界或未放置的房間，則傳回空值。

以下範例收集目前文件和所選視圖中的所有房間，然後傳回中心邊界。
___
## 範例檔案

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)

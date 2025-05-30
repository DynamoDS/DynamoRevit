## 深入資訊
`Space.CoreCenterBoundary` 會傳回表示給定空間核心中心邊界的巢狀清單。在傳回的清單中，第一個子清單表示最外側的曲線，後續的清單表示空間內的迴圈。核心中心邊界位於定義為核心牆的中心處。如需牆定位線的更多資訊，請參閱此說明 [文章](https://help.autodesk.com/view/RVT/2024/CHT/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)。

如果給定未設邊界或未放置的空間，則傳回空值。

以下範例收集目前文件和所選視圖中的所有空間，然後傳回核心中心邊界。

___
## 範例檔案

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)

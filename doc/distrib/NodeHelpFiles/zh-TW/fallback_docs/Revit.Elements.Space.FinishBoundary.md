## 深入資訊
`Space.FinishBoundary` 會傳回表示給定空間完成邊界的巢狀清單。在傳回的清單中，第一個子清單表示最外側的曲線，後續的清單表示空間內的迴圈。此節點傳回的空間邊界位於 Revit 空間內的完成面。如需牆定位線的更多資訊，請參閱此說明 [文章](https://help.autodesk.com/view/RVT/2024/CHT/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)。

如果給定未設邊界或未放置的空間，則傳回空值。

以下範例收集目前文件和所選視圖中的所有空間，然後傳回完成邊界。

___
## 範例檔案

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)

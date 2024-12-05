## 详细
“Room.CoreBoundary”返回代表给定房间核心边界的嵌套列表。在返回的列表中，第一个子列表表示最外边的曲线，而后续列表表示房间内的环路。核心边界位于最靠近房间的核心的内部或外部图层。有关墙定位线的更多信息，请参阅帮助 [文章] (https://help.autodesk.com/view/RVT/2024/CHS/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)。

如果给定了无边界或未放置的房间，则返回空值。

在下面的示例中，将从当前文档和选定视图收集所有房间。然后返回核心边界。
___
## 示例文件

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)

## 详细
“Room.IsInsideRoom”返回一个布尔值，该值指示是否在给定房间中找到给定点。

在下面的示例中，将收集当前 Revit 文档中的所有家具以及所有房间。然后，家具的位置点传递到“Room.IsInsideRoom”中，以检查给定点出现在哪个房间中(如果可用)。最后，过滤家具中具有房间位置的图元，并将这些值组合到列表中。
___
## 示例文件

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

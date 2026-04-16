## 详细
此节点在 Revit 图元的选定面上创建新的分割表面图元，并使用指定的 U 和 V 分割以及可选的旋转角度定义其布局。
分割表面是应用到面上的图案化网格，通常用于在形状表面上定位幕墙嵌板、自适应构件或嵌板系统。

U 向分段数和 V 向分段数确定每个曲面方向上出现的细分数，而旋转参数则相对于曲面的 U-V 坐标系调整栅格方向。

在此示例中，将选择面并将其用作节点 DividedSurface.ByFaceUVDivisionsAndRotation 以及由滑块控制的 UDiv、VDiv 和 gridRotation 的曲面输入。最后的节点会显示分割表面的值。运行此示例图形时，您需要遵照 Revit 警告并删除建议的图元，以便轴网显示在选定表面上。

有关更多信息，请参见链接。
https://help.autodesk.com/view/RVT/2025/CHS/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## 示例文件

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)

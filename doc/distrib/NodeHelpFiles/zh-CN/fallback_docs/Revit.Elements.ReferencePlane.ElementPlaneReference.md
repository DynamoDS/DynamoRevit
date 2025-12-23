## 详细
此节点提取选定参照平面的实际 Revit 图元参照。当需要在 Revit 中将该平面用作几何图形或尺寸标注的主体参照时，此选项非常有用。

示例:
在此图形中，使用坐标定义两个点，并使用 ReferencePlane.ByStartPointEndPoint 在这两个点之间创建参照平面。然后，该参照平面连接到 ReferencePlane.ElementPlaneReference，后者会输出平面的 Revit 原生参照，使其准备好用于主体或对齐任务。
___
## 示例文件

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

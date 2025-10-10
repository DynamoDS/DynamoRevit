## 详细
使用最小点和最大点创建给定任意 CoordinateSystem 的截面视图。

在此示例中，我们生成一个 SectionView，其位置和方向与输入 CoordinateSystem 直接相关。在这里，CoordinateSystem 经过了战略性定位，以定义一个以建筑物右上角的为重点的截面。请务必注意，minPoint 和 maxPoint 输入的 Z 分量精确指定生成的 Revit SectionView 的“远剪裁偏移”参数，从而控制其在模型中的有效查看深度。
有关控制图元显示的详细信息，请参见以下链接
https://help.autodesk.com/view/RVT/2024/CHS/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## 示例文件

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)

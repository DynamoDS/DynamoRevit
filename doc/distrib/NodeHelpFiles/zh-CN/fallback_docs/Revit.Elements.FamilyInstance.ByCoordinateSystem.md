## 详细
在 Revit 项目中放置一个 Revit FamilyInstance，给定所需的 FamilyType 及其坐标系。

在此示例中，将提供特定的族类型和坐标系点，并放置族实例。
常见用例涉及基于 Revit 项目基点创建坐标系，然后旋转该坐标系以匹配场地旋转。然后，可以将此变换后的坐标系输入 FamilyInstance.ByCoordinateSystem 节点，以将族实例放置在其预期的真实世界位置，同时考虑场地方向。

有关坐标系的详细信息，请参见下文。
https://help.autodesk.com/view/RVT/2025/CHS/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## 示例文件

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

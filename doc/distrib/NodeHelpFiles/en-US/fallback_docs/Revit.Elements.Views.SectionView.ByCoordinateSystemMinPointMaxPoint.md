## In Depth
Creates section views given any CoordinateSystem with Min and Max points.

In this example, we generate a SectionView whose placement and orientation are directly correlated with the input CoordinateSystem. Here, that CoordinateSystem has been strategically positioned to define a section focused on the building's upper-right corner. It's crucial to observe that the Z-component of our minPoint & maxPoint input precisely dictates the Far Clip Offset parameter of the resulting Revit SectionView, thereby controlling its effective viewing depth into the model.
For more information on controlling elements display see link below
https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## Example File

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
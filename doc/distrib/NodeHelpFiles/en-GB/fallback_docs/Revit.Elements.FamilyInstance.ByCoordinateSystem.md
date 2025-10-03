## In Depth
Place a Revit FamilyInstance in a Revit project, given the desired FamilyType and its coordinate system.

In this example a specific Family Type and Coordinate system point are provided and a Family Instance is placed.
A common use case involves creating a coordinate system based on the Revit project base point and then rotating it to match the site rotation. You can then feed this transformed coordinate system into the FamilyInstance.ByCoordinateSystem node to place family instances at their intended real-world locations, taking into account the site orientation.

For more information on coordinate systems see below.
https://help.autodesk.com/view/RVT/2025/ENG/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Example File

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

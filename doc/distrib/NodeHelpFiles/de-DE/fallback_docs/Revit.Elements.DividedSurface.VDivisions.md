## Im Detail
This node returns the number of V divisions applied to a given Divided Surface in Revit. The grid is defined in two directions—U and V—and the V divisions determine how many segments the surface is divided into along one axis (typically corresponding to the surface’s parametric “V” direction). In the Revit conceptual massing environment or adaptive component families, a Divided Surface is a patterned grid applied to a face (such as a wall, roof, or form face).

In this example, a face is selected and used to generate a divided surface.  This divided surface is the input for DividedSurface.VDivisions.  The last nodes expose the other values of the divided surface.  When running this example graph, you will need to observe the Revit warning and delete the suggested elements so that the grids will appear on the surface selected.  The number of V divisions applied to the divided surface.

See link for addition information.
https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Beispieldatei

![DividedSurface.VDivisions](./Revit.Elements.DividedSurface.VDivisions_img.jpg)

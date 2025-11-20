## Informacje szczegółowe
This node creates a new Divided Surface element on a selected face of a Revit element and defines its layout using specified U and V divisions.  A Divided Surface is a patterned grid applied to a face, commonly used to position curtain panels, adaptive components, or panelized systems across a form surface.

The U divisions and V divisions determine how many subdivisions occur in each surface direction, while the rotation parameter adjusts the grid orientation relative to the surface’s U-V coordinate system.

In this example, a face is selected and used as an input to surface for node DividedSurface.ByFaceUVDivisions along with UDivs, VDivs which are controlled by sliders.  The last nodes expose the values of the divided surface.  When running this example graph, you will need to observe the Revit warning and delete the suggested elements so that the grids will appear on the surface selected.

See link for addition information.
https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Plik przykładowy

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## Plik przykładowy

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)

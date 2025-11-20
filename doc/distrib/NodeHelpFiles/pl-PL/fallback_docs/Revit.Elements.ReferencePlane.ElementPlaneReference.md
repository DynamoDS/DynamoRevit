## Informacje szczegółowe
This node extracts the actual Revit element reference of a selected reference plane. This is useful when you need to use that plane as a hosting reference for geometry or dimensions inside Revit.

Przykład:
In this graph, two points are defined using coordinates, and a reference plane is created between them with ReferencePlane.ByStartPointEndPoint. That reference plane is then connected to ReferencePlane.ElementPlaneReference, which outputs the plane’s Revit-native reference, making it ready to be used for hosting or alignment tasks.
___
## Plik przykładowy

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

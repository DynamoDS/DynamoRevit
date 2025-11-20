## Em profundidade
This node extracts the underlying geometric plane from a Reference Plane element.

In this graph, two points are defined with coordinates and connected to the ReferencePlane.ByStartPointEndPoint node to create a reference plane. The created reference plane is then passed into the ReferencePlane.Plane node, which outputs its geometric plane, then to a 3D watch node.
___
## Arquivo de exemplo

![ReferencePlane.Plane](./Revit.Elements.ReferencePlane.Plane_img.jpg)

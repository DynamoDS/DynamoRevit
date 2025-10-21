## En detalle:
Coloque un FamilyInstance de Revit en un proyecto de Revit con el FamilyType deseado y su sistema de coordenadas.

En este ejemplo, se proporcionan un tipo de familia y un punto del sistema de coordenadas específicos y se coloca un ejemplar de familia.
Un caso de uso habitual consiste en crear un sistema de coordenadas basado en el punto base del proyecto de Revit y, a continuación, girarlo para que coincida con la rotación del emplazamiento. Después, puede introducir este sistema de coordenadas transformado en el nodo FamilyInstance.ByCoordinateSystem para colocar las instancias de familia en sus ubicaciones reales previstas, teniendo en cuenta la orientación del emplazamiento.

Para obtener más información sobre los sistemas de coordenadas, consulte el vínculo siguiente.
https://help.autodesk.com/view/RVT/2025/ESP/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Archivo de ejemplo

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

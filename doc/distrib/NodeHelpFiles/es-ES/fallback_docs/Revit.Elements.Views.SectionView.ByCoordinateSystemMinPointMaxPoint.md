## En detalle:
Crea vistas en sección a partir de cualquier sistema de coordenadas con puntos mínimo y máximo.

En este ejemplo, se genera una SectionView cuya posición y orientación están directamente relacionadas con el CoordinateSystem de entrada. Aquí, el CoordinateSystem se ha colocado estratégicamente para definir una sección centrada en la esquina superior derecha del edificio. Es crucial observar que el componente Z de la entrada minPoint y maxPoint determina con precisión el parámetro Desfase de delimitación lejano de la SectionView de Revit resultante, lo que permite controlar su profundidad de visualización efectiva en el modelo.
Para obtener más información sobre el control de visualización de elementos, consulte el vínculo siguiente.
https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## Archivo de ejemplo

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)

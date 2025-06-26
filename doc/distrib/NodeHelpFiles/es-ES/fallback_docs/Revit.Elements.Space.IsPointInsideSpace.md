## En detalle:
`Space.IsPointInsideSpace`comprueba si un punto determinado se encuentra dentro de un espacio especificado. Esto puede ser útil al asignar valores de marca a elementos en Revit.

En el ejemplo siguiente, se recopilan todos los terminales de aire de la vista especificada en el documento de Revit actual. A continuación, sus ubicaciones de puntos se comparan con los espacios de la vista indicada a través de `Space.IsPointInsideSpace`. Mediante la gestión de listas, se elaboran sublistas para filtrar los terminales de aire que se encuentran dentro de espacios. Para obtener más información sobre el uso de List@Level, consulte este [artículo](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Archivo de ejemplo

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

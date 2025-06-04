## En detalle:
`Space.FinishBoundary` devuelve una lista anidada que representa el contorno de acabado del espacio especificado. En la lista devuelta, la primera sublista representa las curvas más externas, mientras que las listas posteriores representan los bucles dentro del espacio. El contorno de espacio devuelto por este nodo se encuentra en la cara de acabado dentro del espacio de Revit. Para obtener más información sobre las líneas de ubicación de muros, en la Ayuda, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si se especifica un espacio sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todos los espacios del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos de acabado.

___
## Archivo de ejemplo

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)

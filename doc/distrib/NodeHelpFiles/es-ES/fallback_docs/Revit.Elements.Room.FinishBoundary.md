## En detalle:
`Room.FinishBoundary` devuelve una lista anidada que representa el contorno de acabado de la habitación especificada. En la lista devuelta, la primera sublista representa las curvas más exteriores, mientras que las listas posteriores representan bucles dentro de la habitación. El contorno de la habitación devuelto por este nodo se encuentra en la cara de acabado dentro de la habitación de Revit. Para obtener más información sobre las líneas de ubicación de muros, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) de la Ayuda.

Si se especifica una habitación sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todas las habitaciones del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos de acabado.
___
## Archivo de ejemplo

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)

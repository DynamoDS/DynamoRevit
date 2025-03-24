## En detalle:
`Room.CoreBoundary` devuelve una lista anidada que representa el contorno del núcleo de la habitación especificada. En la lista devuelta, la primera sublista representa las curvas más externas, mientras que las listas posteriores representan bucles dentro de la habitación. Los contornos del núcleo se encuentran en la capa interior o exterior del núcleo más cercana a la habitación. Para obtener más información sobre las líneas de ubicación de muros, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) de la Ayuda.

Si se especifica una habitación sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todas las habitaciones del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos del núcleo.
___
## Archivo de ejemplo

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)

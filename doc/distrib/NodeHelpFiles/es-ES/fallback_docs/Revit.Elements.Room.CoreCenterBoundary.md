## En detalle:
`Room.CoreCenterBoundary` devuelve una lista anidada que representa el contorno del centro del núcleo de la habitación especificada. En la lista devuelta, la primera sublista representa las curvas más exteriores, mientras que las listas siguientes representan bucles dentro de la habitación. Los contornos del centro del núcleo se encuentran en el centro de los muros que se definen como núcleo. Para obtener más información sobre las líneas de ubicación de muros, consulte este [artículo] (https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) de la Ayuda.

Si se especifica una habitación sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todas las habitaciones del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos del centro del núcleo.
___
## Archivo de ejemplo

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)

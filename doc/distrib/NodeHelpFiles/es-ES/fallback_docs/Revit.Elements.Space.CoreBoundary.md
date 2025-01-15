## En detalle:
`Space.CoreBoundary` devuelve una lista anidada que representa el contorno del núcleo del espacio especificado. En la lista devuelta, la primera sublista representa las curvas más externas, mientras que las listas posteriores representan los bucles dentro del espacio. Los contornos del núcleo se encuentran en la capa interior o exterior del núcleo más cercana a la habitación. Para obtener más información sobre las líneas de ubicación de muros, en la Ayuda, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si se especifica un espacio sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todos los espacios del documento actual y de la vista seleccionada. A continuación se devuelven los contornos del núcleo.

___
## Archivo de ejemplo

![Space.CoreBoundary](./Revit.Elements.Space.CoreBoundary_img.jpg)

## En detalle:
`Space.CoreCenterBoundary` devuelve una lista anidada que representa el contorno del centro del núcleo del espacio especificado. En la lista devuelta, la primera sublista representa las curvas más externas, mientras que las listas posteriores representan los bucles dentro del espacio. Los contornos del centro del núcleo se encuentran en el centro de los muros que se han definido como núcleo. Para obtener más información sobre las líneas de ubicación de muros, en la Ayuda, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si se especifica un espacio sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todos los espacios del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos del centro del núcleo.

___
## Archivo de ejemplo

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)

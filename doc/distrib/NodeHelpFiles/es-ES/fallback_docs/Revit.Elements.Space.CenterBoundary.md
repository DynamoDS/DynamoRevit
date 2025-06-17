## En detalle:
`Space.CenterBoundary` devuelve una lista anidada que representa el contorno del eje del espacio especificado. En la lista devuelta, la primera sublista representa las curvas más externas, mientras que las listas siguientes representan bucles dentro del espacio. Los contornos del centro se sitúan en el eje del muro a través de todas las capas dentro del espacio de Revit. Para obtener más información sobre las líneas de ubicación de muros, en la Ayuda, consulte este [artículo](https://help.autodesk.com/view/RVT/2024/ESP/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si se especifica un espacio sin delimitar o sin colocar, se devuelve un valor nulo.

En el ejemplo siguiente, se recopilan todos los espacios del documento actual y de la vista seleccionada. A continuación, se devuelven los contornos del centro.
___
## Archivo de ejemplo

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)

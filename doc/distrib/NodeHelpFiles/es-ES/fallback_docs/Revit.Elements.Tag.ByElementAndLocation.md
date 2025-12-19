## En detalle:
Este nodo etiqueta elementos de Revit en función de una vista, un elemento, una ubicación, una horizontal (si no, la etiqueta se orientará en función del elemento) y addLeader como entradas.

En este ejemplo, se selecciona una puerta en la vista "Studio Live Work Core B". La ubicación de esa puerta se extrae y, a continuación, se utiliza como entrada original en Tag.ByElementAndLocation junto con los valores booleanos de horizontal y addLeader. La ubicación original se modifica para que la ubicación de la etiqueta no se superponga directamente sobre el elemento mediante el nodo Tag.SetHeadLocation.

___
## Archivo de ejemplo

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

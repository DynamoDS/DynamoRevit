## En detalle:
Este nodo toma una etiqueta y cambia su ubicación de encabezamiento. Esto nos permite automatizar un comportamiento de colocación coherente para que las etiquetas estén directamente encima del elemento que etiquetan.

En este ejemplo, se selecciona una puerta en la vista "Studio Live Work Core B". La ubicación de esa puerta se extrae y, a continuación, se utiliza como entrada original en Tag.ByElementAndLocation junto con los valores booleanos de horizontal y addLeader. La ubicación original se modifica para que la ubicación de la etiqueta no se superponga directamente sobre el elemento mediante el nodo Tag.SetHeadLocation.

___
## Archivo de ejemplo

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

## En detalle:
`Element.SetParameterByName` establece un elemento de parámetro (encontrado por nombre) en el valor especificado. Entre los valores se incluyen los siguientes: doble, entero, booleano, ID de elemento, elemento y cadena. En Revit, los parámetros pueden compartir el mismo nombre. Como resultado, `Element.SetParameterByName` establecerá el valor en el primer parámetro que se encuentre, ordenado alfabéticamente por ID exclusivo.

En el ejemplo siguiente, el parámetro de comentarios se establece para todos los elementos de mobiliario del modelo que se encuentran dentro de una habitación. El valor de este parámetro es el nombre de la habitación que se obtiene.
___
## Archivo de ejemplo

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

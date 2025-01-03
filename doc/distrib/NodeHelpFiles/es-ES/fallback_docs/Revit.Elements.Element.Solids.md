## En detalle:
`Element.Solids` devuelve la geometría de sólidos del elemento especificado. Los sólidos se devuelven como listas anidadas, ya que cualquier elemento especificado puede tener más de un sólido en su interior. Si se desea obtener un único sólido que represente al elemento, se puede utilizar `Solid.ByUnion` en las listas.

En el ejemplo siguiente, se recopilan todos los muros de la vista seleccionada. A continuación, se eliminan los muros creados como familias in situ y se devuelven los sólidos de los muros restantes.

___
## Archivo de ejemplo

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

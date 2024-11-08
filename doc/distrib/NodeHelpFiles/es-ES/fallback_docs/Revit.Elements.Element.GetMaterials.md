## En detalle:
`Element.GetMaterials` devuelve todos los materiales _(y sus ID)_ que existen en un elemento de Revit. Los elementos con varios materiales devolverán una lista para cada elemento. La entrada, "paintMaterials", es un conmutador booleano que indica al nodo que recopile también los materiales sobre los que pinte el usuario.

En el ejemplo siguiente, se devuelven los materiales de todos los ejemplares de muro del documento (archivo) actual.
___
## Archivo de ejemplo

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)

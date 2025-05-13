## En detalle:
`Level.ProjectElevation` devuelve la elevación del nivel especificado en unidades de proyecto. `Level.ProjectElevation` informa del valor desde el origen del proyecto. Si se requiere la elevación desde el nivel del suelo, este valor puede obtenerse con `Level.Elevation`.

En el ejemplo siguiente, se recopilan todos los niveles en el documento de Revit actual. Se devuelve el valor de elevación del proyecto de los niveles. Además, los niveles se ordenan por altura con`List.SortByKey`.
___
## Archivo de ejemplo

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)

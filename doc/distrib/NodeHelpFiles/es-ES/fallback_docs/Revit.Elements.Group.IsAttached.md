## En detalle:
Determina si un elemento de grupo especificado del proyecto de Revit está enlazado a un grupo principal. En términos más sencillos, comprueba si un grupo forma parte de un grupo anidado de mayor tamaño. Este nodo es útil cuando necesita identificar y administrar grupos organizados dentro de otros grupos. Por ejemplo, puede utilizarlo para lo siguiente:
• Filtrar grupos: aísle los grupos que no forman parte de otros grupos.
• Administrar grupos anidados: conozca la estructura de los grupos anidados y procéselos en consecuencia.
• Modificación de elementos de control: evite realizar cambios directos en los elementos de un grupo que esté enlazado a un grupo principal, ya que esto podría alterar la estructura del grupo principal.
• Automatizar tareas: gestione y manipule dinámicamente los grupos en función de si están enlazados o no.
Básicamente, el nodo Group.IsAttached ayuda a comprender la relación entre los grupos del modelo de Revit y a generar flujos de trabajo de Dynamo que tengan en cuenta esta jerarquía.

En el ejemplo siguiente, todos los grupos de modelo se recopilan del documento de Revit activo como entrada. Las salidas son "True" (verdadero) o "False" (falso). Los resultados "true" indican que el grupo tiene enlaces (anidamiento). Los resultados "false" indican que el grupo NO tiene enlaces (anidamiento).

___
## Archivo de ejemplo

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)

## En detalle:
Elimina todos los filtros definidos de una entrada especificada de una ScheduleView.

En este ejemplo, definimos inicialmente una vista de tabla de planificación que se enruta directamente a ScheduleView.ScheduleFilters para mostrar explícitamente sus filtros de vista preexistentes. Para resaltar la transformación, esta misma vista pasa por una breve pausa de 10 milisegundos. Tras este breve intervalo, la vista pasa al nodo ScheduleView.ClearAllFilters, que elimina todos los filtros aplicados. Por último, la vista modificada se canaliza de nuevo a otro nodo ScheduleView.ScheduleFilters, lo que demuestra de forma inequívoca que su lista de filtros se ha convertido en una lista vacía. Esto confirma que se han borrado correctamente todos los filtros originales.
___
## Archivo de ejemplo

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

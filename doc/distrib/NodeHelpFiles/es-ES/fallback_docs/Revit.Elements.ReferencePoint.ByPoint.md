## En detalle:
"ReferencePoint.ByPoint" crea un elemento de punto de referencia en el documento de familia de Revit activo en la ubicación de punto especificada. Nota: El documento de familia debe ser un componente adaptativo o una familia de masas. Este nodo se diferencia de "ReferencePoint.ByCoordinates" en que utiliza un punto de Dynamo para la ubicación. Esto es útil porque el usuario final puede utilizar la manipulación directa para editar el punto de Dynamo, lo que también permite actualizar el punto de referencia. Para obtener más información sobre la manipulación directa, visite este [enlace](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

En el siguiente ejemplo, se crea un nuevo punto de referencia en las coordenadas 0,0,1.
___
## Archivo de ejemplo

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

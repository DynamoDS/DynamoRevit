## En detalle:
El nodo Revision.SetIssuedBy de Dynamo se utiliza para establecer o actualizar el valor "Emitida por" de una revisión en Revit. Ayuda a automatizar el control de revisiones registrando quién emitió la revisión, lo que garantiza que la documentación sea clara y coherente sin necesidad de realizar ediciones manuales en Revit.

En este gráfico, el nodo Select Revision se utiliza para seleccionar la revisión necesaria, y una entrada de cadena (p. ej., ABC) proporciona el nombre del emisor. A continuación, el nodo Revision.SetIssuedBy aplica este valor a la revisión seleccionada y actualiza el campo "Emitida por" directamente en el modelo de Revit.

___
## Archivo de ejemplo

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)

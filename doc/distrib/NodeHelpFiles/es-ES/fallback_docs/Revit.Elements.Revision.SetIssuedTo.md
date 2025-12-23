## En detalle:
El nodo Revision.SetIssuedTo se utiliza para establecer o actualizar el valor "Emitida a" de una revisión en Revit. Ayuda a automatizar el proceso de registro del destinatario de una revisión, lo que garantiza la precisión y la coherencia de la documentación del proyecto sin necesidad de realizar entradas manuales en Revit.

En este gráfico, el nodo Select Revision se utiliza para elegir la revisión necesaria, mientras que una entrada de cadena (p. ej., XYZ) define al destinatario. A continuación, el nodo Revision.SetIssuedTo aplica este valor a la revisión seleccionada y actualiza el campo "Emitida a" directamente en el modelo de Revit.
___
## Archivo de ejemplo

![Revision.SetIssuedTo](./Revit.Elements.Revision.SetIssuedTo_img.jpg)

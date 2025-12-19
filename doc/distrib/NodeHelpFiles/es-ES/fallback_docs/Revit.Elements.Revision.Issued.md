## En detalle:
El nodo Revision.Issued de Dynamo se utiliza para comprobar si una revisión de Revit se ha marcado como emitida. Devuelve un valor (booleano) verdadero (true) o falso (false), lo que ayuda a los equipos a verificar rápidamente el estado de las revisiones sin abrir la configuración de revisión de Revit.

En este gráfico, el nodo Select Revision se utiliza para elegir una revisión del proyecto. A continuación, el nodo Revision.Issued comprueba si la revisión seleccionada se ha emitido y el resultado se muestra en el nodo Watch como verdadero (true) o falso (false). Esto facilita la confirmación del estado de emisión de una revisión directamente a través de Dynamo.

___
## Archivo de ejemplo

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)

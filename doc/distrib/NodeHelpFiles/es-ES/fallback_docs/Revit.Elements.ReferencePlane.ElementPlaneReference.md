## En detalle:
Este nodo extrae la referencia real del elemento de Revit de un plano de referencia seleccionado. Esto resulta útil cuando se debe utilizar ese plano como referencia de anfitrión para la geometría o las cotas en Revit.

Ejemplo:
En este gráfico, se definen dos puntos mediante coordenadas y se crea un plano de referencia entre ellos con ReferencePlane.ByStartPointEndPoint. A continuación, ese plano de referencia se conecta a ReferencePlane.ElementPlaneReference, que genera la referencia nativa de Revit del plano, lo que lo prepara para su uso en tareas de hospedaje o alineación.
___
## Archivo de ejemplo

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

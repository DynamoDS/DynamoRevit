## En detalle:
`Rooms By Status` devuelve todas las habitaciones del documento agrupadas por estado.

El estado incluye los siguientes tipos de habitaciones:
- Habitaciones colocadas y delimitadas correctamente
- Habitaciones no colocadas (eliminadas anteriormente de una vista, pero no del modelo)
- Habitaciones no cerradas (no delimitadas sin superficie calculada)
- Habitaciones redundantes (que comparten un espacio cerrado con otras habitaciones)

En el ejemplo siguiente, se consulta el área de las habitaciones colocadas.
___
## Archivo de ejemplo

![Rooms By Status](./DSRevitNodesUI.RoomsByStatus_img.jpg)

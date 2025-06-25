## En detalle
"Room.IsInsideRoom" devuelve un valor booleano que indica si el punto especificado se encuentra en la habitación especificada.

En el siguiente ejemplo, se recopila todo el mobiliario del documento de Revit actual, junto con todas las habitaciones. A continuación, los puntos de ubicación del mobiliario se pasan a "Room.IsInsideRoom" para comprobar en qué habitación se encuentran los puntos especificados (si están disponibles). Por último, el mobiliario se filtra en busca de elementos con ubicaciones de habitación, y estos valores se combinan en listas.
___
## Archivo de ejemplo

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

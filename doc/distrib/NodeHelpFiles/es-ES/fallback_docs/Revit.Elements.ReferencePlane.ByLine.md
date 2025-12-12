## En detalle:
El nodo ReferencePlane.ByLine de Dynamo crea un plano de referencia de Revit mediante el uso de una línea definida como base. Esto permite generar planos de referencia personalizados en posiciones y orientaciones específicas.

En este ejemplo, se definen dos puntos mediante Point.ByCoordinates con controles deslizantes ajustables. A continuación, se crea un nodo Line.ByStartPointEndPoint entre estos dos puntos y, por último, el nodo ReferencePlane.ByLine genera un plano de referencia a lo largo de esa línea.
___
## Archivo de ejemplo

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

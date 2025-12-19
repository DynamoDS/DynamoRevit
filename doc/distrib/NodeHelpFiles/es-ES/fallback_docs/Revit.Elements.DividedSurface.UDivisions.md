## En detalle:
Este nodo devuelve el número de divisiones en U aplicadas a una superficie dividida de Revit determinada. La rejilla se define en dos direcciones, U y V, y las divisiones en U determinan en cuántos segmentos se divide la superficie a lo largo de un eje (normalmente corresponde a la dirección paramétrica "U" de la superficie). En el entorno de masas conceptuales o familias de componentes adaptativos de Revit, una superficie dividida es una rejilla con patrón que se aplica a una cara (por ejemplo, un muro, una cubierta o una cara de forma).

En este ejemplo, se selecciona una cara y se utiliza para generar una superficie dividida. Esta superficie dividida es la entrada de DividedSurface.UDivisions. Los últimos nodos exponen los demás valores de la superficie dividida. Al ejecutar este gráfico de ejemplo, deberá observar la advertencia de Revit y suprimir los elementos sugeridos para que las rejillas aparezcan en la superficie seleccionada. El número de divisiones en U aplicadas a la superficie dividida.

Consulte el vínculo para obtener más información.
https://help.autodesk.com/view/RVT/2025/ESP/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Archivo de ejemplo

![DividedSurface.UDivisions](./Revit.Elements.DividedSurface.UDivisions_img.jpg)

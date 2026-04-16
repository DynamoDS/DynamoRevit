## En detalle:
Este nodo crea un nuevo elemento de superficie dividida en una cara seleccionada de un elemento de Revit y define su diseño mediante las divisiones U y V especificadas. Una superficie dividida es una rejilla con patrón aplicada a una cara que se utiliza normalmente para colocar paneles de muro cortina, componentes adaptativos o sistemas panelizados en una superficie de forma.

Las divisiones U y V determinan el número de subdivisiones que hay en cada dirección de superficie, mientras que el parámetro de rotación ajusta la orientación de la rejilla en relación con el sistema de coordenadas U-V de la superficie.

En este ejemplo, se selecciona una cara y se utiliza como entrada para la superficie del nodo DividedSurface.ByFaceUVDivisions junto con UDivs y VDivs, que se controlan mediante controles deslizantes. Los últimos nodos exponen los valores de la superficie dividida. Al ejecutar este gráfico de ejemplo, deberá observar la advertencia de Revit y suprimir los elementos sugeridos para que las rejillas aparezcan en la superficie seleccionada.

Consulte el vínculo para obtener más información.
https://help.autodesk.com/view/RVT/2025/ESP/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Archivo de ejemplo

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## Archivo de ejemplo

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)

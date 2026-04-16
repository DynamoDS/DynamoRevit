## En detalle:
Este nodo realiza un análisis de rebote de rayo en el modelo de Revit. Comienza en un punto de origen determinado y se desplaza a lo largo de un vector de dirección especificado para trazar la trayectoria del rayo a medida que interseca elementos en el modelo. Cuando el rayo incide en una superficie, puede rebotar de nuevo en esa superficie, según el número de rebotes permitidos, simulando el comportamiento de la luz, la visibilidad o la reflexión de la ruta.

En este ejemplo, se selecciona un elemento y se utiliza su ubicación para el origen de entrada del nodo RayBounce.ByOriginDirection, junto con una dirección, maxBounces y una vista.
___
## Archivo de ejemplo

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)

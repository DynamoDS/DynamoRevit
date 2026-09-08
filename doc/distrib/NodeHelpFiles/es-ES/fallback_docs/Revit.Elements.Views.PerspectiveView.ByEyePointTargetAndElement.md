## En detalle:
`PerspectiveView.ByEyePointTargetAndElement` crea una nueva vista 3D en perspectiva de Revit mediante un punto de ocular, un punto de destino, un elemento, un nombre y un valor booleano `isolateElement`.

En el ejemplo siguiente, se selecciona un elemento de Revit y se utiliza como punto de referencia de la vista en perspectiva. Se crea un punto para la ubicación de la cámara y otro para la ubicación de destino. Estos se utilizan como parámetros de entrada para `PerspectiveView.ByEyePointTargetAndElement`, junto con el elemento seleccionado. El resultado es una nueva vista en perspectiva desde el punto de ocular hacia el punto de destino con la extensión de la vista basada en el elemento seleccionado.
___
## Archivo de ejemplo

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)

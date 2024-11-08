## En detalle:
Cada vista de Revit tiene un origen. `View.Origin` devuelve este valor como un punto de Dynamo. Según la documentación de la API de Revit, "El origen de una vista de plano no es significativo". Teniendo esto en cuenta, el valor ofrecido por `View.Origin' depende del usuario final.

En el ejemplo siguiente, se devuelve el origen de la vista activa y una vista 3D seleccionada.
___
## Archivo de ejemplo

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)

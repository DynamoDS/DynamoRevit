## En detalle:
`Subelement.GetAllParameters` recupera los ID de parámetro internos de Revit de los subelementos, no los nombres para mostrar de esos parámetros.

En el ejemplo siguiente, se seleccionan todos los elementos de la vista actual y se filtran para mostrar solo aquellos que contienen subelementos. A continuación, los subelementos se utilizan como entrada para `Subelement.GetAllParameters`. El resultado es una lista de los ID de parámetro asociados a cada subelemento. El último nodo muestra cómo se utiliza ese resultado para recuperar los valores de los parámetros.

___
## Archivo de ejemplo

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)

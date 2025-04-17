## En detalle:
`Dimension.ValueOverride` devuelve la modificación de valor de la cota especificada si tiene un valor modificado. En las cotas de varios segmentos, se devuelve una lista anidada de valores. Si la cota no tiene un valor modificado, se devuelve un valor "null".

En el ejemplo siguiente, se recopila la primera cota de la vista activa y se comprueba si contiene una cota modificada. Si es así, esta se borra con el comando `Dimension.SetValueOverride`.
___
## Archivo de ejemplo

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)

## En detalle:
`Element.Delete` funciona de la misma forma que la opción de supresión en la interfaz de Revit. Suprimirá el elemento de entrada y todos los elementos que dependen de él.

Por ejemplo, al suprimir un muro con puertas, se suprimirán también las puertas. La salida consta de una lista anidada de los ID de los elementos que se han suprimido en consecuencia. Nota: Este nodo se utiliza de forma más eficaz en el modo de ejecución manual en Dynamo.

En el ejemplo siguiente, se suprimen todos los ejemplares de la familia "Botón de ayuda" del documento (archivo) actual.
___
## Archivo de ejemplo

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

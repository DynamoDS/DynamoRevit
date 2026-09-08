## En detalle:
`Remember` almacena un valor en el archivo de Dynamo para que el gráfico pueda recuperarlo más adelante, incluso aunque la entrada sea nula. Resulta útil para conservar datos entre ejecuciones, mantener el estado o consultar información almacenada anteriormente sin necesidad de volver a generarla cada vez.

En el ejemplo siguiente, se selecciona un único suelo y se extrae la geometría, que se utiliza como entrada para `Remember`. El resultado almacenado queda entonces disponible como salida para su reutilización en el gráfico.
___
## Archivo de ejemplo

![Remember](./CoreNodeModels.Remember_img.jpg)

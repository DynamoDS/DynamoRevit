## En detalle:
`Gate` controla si se permite el paso de los datos de entrada en función de un valor booleano. Si se permite el paso ("True"), la entrada se transmite a la salida. Si no se permite ("False"), la entrada queda bloqueada.

En el ejemplo siguiente, se utiliza `Gate` para evitar que la geometría se descomponga en cada ejecución del gráfico. Si se permite el paso ("True"), la geometría recién creada pasa a través de ella y se descompone. Si no se permite el paso ("False"), la geometría queda bloqueada y no se transmite ni se vuelve a descomponer.
___
## Archivo de ejemplo

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)

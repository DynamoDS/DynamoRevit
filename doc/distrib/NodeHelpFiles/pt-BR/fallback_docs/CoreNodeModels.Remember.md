## Em profundidade
`Remember` armazena um valor no arquivo do Dynamo para que ele possa ser chamado mais tarde pelo gráfico, inclusive quando a entrada é nula. É útil para preservar dados entre execuções, manter o estado ou fazer referência a informações previamente armazenadas sem recriá-las a cada vez.

No exemplo abaixo, é selecionado um único piso e é extraída a geometria e usada como entrada para `Remember`. O resultado armazenado fica disponível como saída para reutilização no gráfico.
___
## Arquivo de exemplo

![Remember](./CoreNodeModels.Remember_img.jpg)

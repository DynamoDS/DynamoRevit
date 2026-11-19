## Em profundidade
`Gate` controla se os dados de entrada podem passar com base em um valor booleano. Se a porta estiver aberta (True), a entrada será passada para a saída. Se a porta estiver fechada (False), a entrada será bloqueada.

No exemplo abaixo, `Gate` é usado para evitar que a geometria seja explodida em cada execução de gráfico. Quando a entrada está aberta (True), a geometria recém-criada passa por ela e é explodida. Quando a entrada está fechada (False ), a geometria é bloqueada e não é passada adiante nem explodida novamente.
___
## Arquivo de exemplo

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)

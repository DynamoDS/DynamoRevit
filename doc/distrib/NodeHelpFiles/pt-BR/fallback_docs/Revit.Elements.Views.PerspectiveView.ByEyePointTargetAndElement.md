## Em profundidade
`PerspectiveView.ByEyePointTargetAndElement` cria uma nova vista 3D em perspectiva no Revit usando um ponto de vista, um ponto de destino, um elemento, um nome e um valor booleano `isolateElement`.

No exemplo abaixo, é selecionado um elemento do Revit e usado como o foco da vista em perspectiva. É criado um ponto para a localização da câmera e outro para a localização de destino. Eles são usados como entradas para `PerspectiveView.ByEyePointTargetAndElement` junto com o elemento selecionado. A saída é uma nova vista em perspectiva partindo do ponto de vista em direção ao ponto alvo, com as extensões da vista com base no elemento selecionado.
___
## Arquivo de exemplo

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)

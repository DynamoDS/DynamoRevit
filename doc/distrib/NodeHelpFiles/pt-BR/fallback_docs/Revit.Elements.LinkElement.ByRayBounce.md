## Em profundidade
Esse nó projeta um raio em um modelo vinculado do Revit de uma origem e direção especificadas e, em seguida, traça sucessivos elementos vinculados. Cada elemento representa um ponto onde o raio efetua a interseção com a geometria no modelo vinculado, até um número máximo definido de reflexões.

Neste exemplo, um elemento vinculado é selecionado e a localização desse elemento é usada como a entrada de origem para LinkElement.ByRayBounce junto com uma direção, maxBounces e uma vista. As saídas são pontos e elementos vinculados.
___
## Arquivo de exemplo

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)

## Em profundidade
Esse nó recupera a matriz de transformação aplicada a um elemento de vínculo do Revit no modelo hospedeiro.
Em outras palavras, retorna a transformação de posição, rotação e dimensionamento que mapeia o sistema de coordenadas do elemento vinculado para o sistema de coordenadas do projeto hospedeiro do Revit.
Isso é útil quando você precisa alinhar, analisar ou manipular a geometria entre modelos vinculados.

Neste exemplo, todos os elementos vinculados do Revit no nível L3 são selecionados e inseridos em LinkElement.LinkTransform. A saída é a transformação de posição, rotação e escala do elemento vinculado.
___
## Arquivo de exemplo

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

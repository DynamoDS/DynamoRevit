## Em profundidade
Esse nó recupera a matriz de transformação inversa de um determinado elemento de vínculo do Revit. No Revit, os modelos vinculados podem ser posicionados com transformações (translação, rotação, escala). Esse nó permite obter o inverso dessa transformação, convertendo efetivamente as coordenadas do espaço do modelo vinculado de volta para o sistema de coordenadas do modelo hospedeiro do Revit.

Neste exemplo, todos os elementos vinculados do Revit no nível L3 são selecionados e inseridos em LinkElement.LinkInverseTransform. A saída é o sistema de coordenadas do modelo hospedeiro do Revit.
___
## Arquivo de exemplo

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

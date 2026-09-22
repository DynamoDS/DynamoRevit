## Em profundidade
`ElementFaceReference.BySurface` cria uma referência de face do Revit com base em uma superfície selecionada ou gerada. Ela converte uma superfície do Dynamo associada à geometria do Revit em uma `ElementFaceReference`. Essa referência pode ser usada por outros nós que precisam de uma referência de uma face do Revit em vez de apenas a geometria da superfície.

No exemplo abaixo, é criada uma parede e uma superfície dessa parede é usada como entrada para `ElementFaceReference.BySurface`. A saída é um `ElementFaceReference` que pode ser usado por nós que requerem uma referência a uma face de elemento do Revit.
___
## Arquivo de exemplo

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

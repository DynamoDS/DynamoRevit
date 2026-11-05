## Em profundidade
`Subelement.GetAllParameters` recupera as IDs internas dos parâmetros do Revit dos subelementos, não os nomes de exibição dos parâmetros.

No exemplo abaixo, são selecionados todos os elementos na vista atual e filtrados para mostrar somente os elementos que contêm subelementos. Os subelementos são usados como entrada para `Subelement.GetAllParameters`. A saída é uma lista de IDs de parâmetro associados com cada subelemento. O último nó mostra a saída que está sendo usada para recuperar os valores de parâmetro.

___
## Arquivo de exemplo

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)

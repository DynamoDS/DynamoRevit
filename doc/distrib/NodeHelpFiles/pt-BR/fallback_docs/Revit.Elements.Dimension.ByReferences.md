## Em profundidade
`Dimension.ByReferences` coloca uma cota ao longo da linha de entrada na vista selecionada, usando as referências fornecidas. A saída é um elemento de cota do Revit.

No exemplo abaixo, são criados vários pontos e conectados com nós de linha para gerar curvas de modelo no Revit. As curvas do modelo são convertidas em referências de curva do Revit. As referências são combinadas em uma lista e usadas como entrada `references` para `Dimension.ByReferences`. São criadas linhas adicionais para definir o posicionamento e a direção das cotas. A vista ativa do Revit também é fornecida como a entrada `view`.
___
## Arquivo de exemplo

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

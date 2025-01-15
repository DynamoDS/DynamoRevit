## Em profundidade
`Space.FinishBoundary` retorna uma lista aninhada representando o limite de acabamento do espaço especificado. Na lista retornada, a primeira sublista representa as curvas mais externas, enquanto as listas subsequentes representam contornos dentro do espaço. O limite de espaço retornado por esse nó está localizado na face de acabamento dentro do espaço Revit. Para obter mais informações sobre as linhas de localização de paredes, consulte esta ajuda [artigo](https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se um espaço não delimitado ou não colocado for fornecido, será retornado um valor nulo.

No exemplo abaixo, são coletados todos os espaços do documento atual e da vista selecionada. Em seguida, são retornados os limites de acabamento.

___
## Arquivo de exemplo

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)

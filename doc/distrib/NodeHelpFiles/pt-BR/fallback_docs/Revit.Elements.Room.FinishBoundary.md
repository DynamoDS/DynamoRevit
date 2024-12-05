## Em profundidade
`Room.FinishBoundary` retorna uma lista aninhada representando o limite de acabamento do ambiente fornecido. Na lista retornada, a primeira sublista representa as curvas mais externas, enquanto as listas subsequentes representam contornos dentro do ambiente. O limite do ambiente retornado por esse nó está localizado na face de acabamento dentro do ambiente do Revit. Para obter mais informações sobre as linhas de localização de parede, consulte a ajuda em [artigo](https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se um ambiente não delimitado ou não colocado for fornecido, será retornado um valor nulo.

No exemplo abaixo, são coletados todos os ambientes do documento atual na vista selecionada. São retornados os limites de acabamento.
___
## Arquivo de exemplo

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)

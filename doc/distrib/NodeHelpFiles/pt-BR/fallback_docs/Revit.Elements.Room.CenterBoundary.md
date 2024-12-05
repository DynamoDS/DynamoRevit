## Em profundidade
`Room.CenterBoundary` retorna uma lista aninhada representando o limite da linha central do ambiente fornecido. Na lista retornada, a primeira sublista representa as curvas mais externas, enquanto as listas subsequentes representam contornos dentro do ambiente. Os limites centrais estão localizados na linha central da parede em todas as camadas dentro do ambiente do Revit. Para obter mais informações sobre as linhas de localização de parede, consulte a ajuda em [artigo](https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se um ambiente não delimitado ou não colocado for fornecido, será retornado um valor nulo.

No exemplo abaixo, são coletados todos os ambientes do documento atual na vista selecionada. São retornados os limites centrais.
___
## Arquivo de exemplo

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)

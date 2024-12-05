## Em profundidade
`Room.CoreCenterBoundary` retorna uma lista aninhada representando o limite central do núcleo do ambiente fornecido. Na lista retornada, a primeira sublista representa as curvas mais externas, enquanto as listas subsequentes representam contornos dentro do ambiente. Os limites centrais do núcleo estão localizados no centro das paredes que são definidas como núcleo. Para obter mais informações sobre as linhas de localização de parede, consulte a ajuda em [artigo](https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se um ambiente não delimitado ou não colocado for fornecido, será retornado um valor nulo.

No exemplo abaixo, são coletados todos os ambientes do documento atual na vista selecionada. São retornados os limites centrais do núcleo.
___
## Arquivo de exemplo

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)

## Em profundidade
`Space.CoreCenterBoundary` retorna uma lista aninhada representando o limite do centro do núcleo do espaço especificado. Na lista retornada, a primeira sublista representa as curvas mais externas, enquanto as listas subsequentes representam contornos dentro do espaço. Os limites do centro do núcleo estão localizados no centro das paredes que são definidas como núcleo. Para obter mais informações sobre as linhas de localização de paredes, consulte esta ajuda [artigo](https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Se um espaço não delimitado ou não colocado for fornecido, será retornado um valor nulo.

No exemplo abaixo, são coletados todos os espaços do documento atual e da vista selecionada. Em seguida, são retornados os limites do centro do núcleo.

___
## Arquivo de exemplo

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)

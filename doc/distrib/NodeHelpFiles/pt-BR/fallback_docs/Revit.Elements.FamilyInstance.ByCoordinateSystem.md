## Em profundidade
Coloque um nó FamilyInstance do Revit em um projeto do Revit, dado o FamilyType desejado e seu sistema de coordenadas.

Neste exemplo, um Tipo de família específico e um Ponto do sistema de coordenadas são fornecidos e uma instância da família é colocada.
Um caso de uso comum envolve a criação de um sistema de coordenadas com base no ponto base do projeto do Revit e sua rotação para corresponder à rotação do terreno. Em seguida, é possível alimentar esse sistema de coordenadas transformado no nó FamilyInstance.ByCoordinateSystem para colocar instâncias de família em suas localizações reais pretendidas, levando em consideração a orientação do terreno.

Para obter mais informações sobre os sistemas de coordenadas, veja abaixo.
https://help.autodesk.com/view/RVT/2025/PTB/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Arquivo de exemplo

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

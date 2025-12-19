## Em profundidade
Esse nó coloca componentes adaptativos aplicando valores de parâmetro UV a uma face selecionada, definindo as localizações de colocação para o tipo de família adaptativa.

Neste exemplo, é criada uma superfície dentro da família de massa efetuando a extrusão de uma curva (isso é feito manualmente), e essa superfície é selecionada como a entrada de face. Em seguida, são fornecidos valores UV para determinar as posições de colocação e a família “Diagnostic Tripod-1point.rfa” é usada como o tipo. O nó AdaptiveComponent.ByParametersOnFace gera os componentes adaptativos posicionados na face selecionada. Observe que o “Diagnostic Tripod-1point.rfa” precisa ser carregado na família de massa antes de executar esse gráfico.

Para que esse arquivo de exemplo de ajuda do nó seja executado, é necessário carregar “Diagnostics Tripod-1 point.rfa” no arquivo do Revit. A família é armazenada aqui. C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## Arquivo de exemplo

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)

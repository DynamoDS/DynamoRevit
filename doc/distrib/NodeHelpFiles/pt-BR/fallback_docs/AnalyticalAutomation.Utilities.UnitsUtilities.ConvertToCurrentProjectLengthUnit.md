## Em profundidade
Esse nó assume um valor de comprimento numérico e um identificador de tipo de unidade, convertendo o valor de entrada nas unidades de comprimento do projeto Revit ativo. A saída é um valor duplo que representa o resultado convertido.

Neste exemplo, um controle deslizante de número fornece um valor de comprimento, e uma unidade (por exemplo, metros) é selecionada para obter sua sequência de caracteres Unit.TypeId. Ambos estão conectados ao nó UnitsUtilities.ConvertToCurrentProjectLengthUnit, que retorna o valor do comprimento convertido com base nas configurações de unidade do projeto.
___
## Arquivo de exemplo

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

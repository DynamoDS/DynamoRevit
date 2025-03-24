## Em profundidade
Dado um elemento que suporta a hospedagem de elementos _(por exemplo, paredes)_, `Element.GetHostedElements` retorna os elementos que dependem do elemento fornecido. Por padrão, são retornadas as instâncias de família que são hospedadas no elemento. `Element.GetHostedElements` oferece a opção de incluir outros tipos de elementos hospedados.

Esses elementos incluem:
- aberturas
- elementos que são hospedados em hospedeiros unidos
- paredes incorporadas _(ou seja, paredes cortina)_
- inserções incorporadas compartilhadas

No exemplo abaixo, são coletados todos os elementos de parede de L2. Em seguida, os elementos de parede são verificados quanto a elementos hospedados com `Element.GetHostedElements`. Essa lista é usada para criar duas listas. Paredes com portas e paredes sem portas.
___
## Arquivo de exemplo

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

## Em profundidade
`Room.IsInsideRoom`retorna um valor booleano que indica se o ponto fornecido é encontrado ou não no ambiente especificado.

No exemplo abaixo, são coletados todos os móveis no documento do Revit atual, juntamente com todos os ambientes. Em seguida, os pontos de localização do mobiliário são passados para `Room.IsInsideRoom` para verificar em qual ambiente os pontos especificados ocorrem (se disponíveis). Finalmente, o mobiliário é filtrado para elementos com localizações de ambiente, e esses valores são combinados em listas.
___
## Arquivo de exemplo

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

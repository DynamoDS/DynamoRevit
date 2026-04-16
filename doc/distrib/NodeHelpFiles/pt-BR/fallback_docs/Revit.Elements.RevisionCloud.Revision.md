## Em profundidade
Esse nó extrai o elemento de revisão vinculado a uma nuvem de revisão específica no Revit. Ele fornece os dados de revisão associados a essa nuvem, permitindo que os usuários verifiquem, rastreiem ou validem detalhes de revisão programaticamente em seus projetos.

Neste exemplo, é criado um retângulo com controles deslizantes de número para largura e comprimento, explodido em curvas e invertido para a orientação adequada. Essas curvas, junto com uma vista escolhida (L1_SD) e uma revisão selecionada (Seq. 2 – Não para construção), são usadas para gerar uma nuvem de revisão por meio do nó RevisionCloud.ByCurve. A nuvem de revisão resultante é conectada ao nó RevisionCloud.Revision, que recupera e gera a revisão associada àquela nuvem. Isso garante que os usuários possam confirmar qual revisão está vinculada a cada nuvem de revisão.
___
## Arquivo de exemplo

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)

## Em profundidade
Esse nó extrai os contornos de curva (normalmente, arcos e linhas) que compõem o perímetro visível de uma Nuvem de revisão. Cada segmento da nuvem é representado como um objeto de curva (geralmente, um arco) correspondente à forma “em bolhas” da marca de revisão em uma vista ou folha.

Neste exemplo, é criado um retângulo usando controles deslizantes de número para definir suas dimensões, que são explodidos em curvas e invertidos para orientação. Essas curvas, junto com uma vista selecionada (planta do terreno) e a revisão (seq. 2 – Não para construção), são usadas para gerar uma nuvem de revisão com o nó RevisionCloud.ByCurve. A nuvem de revisão criada é, em seguida, conectada ao nó RevisionCloud.Curves, que extrai e exibe as curvas de definição daquela nuvem. Isso ajuda os usuários a verificar a geometria da nuvem de revisão e fornece flexibilidade para reutilização ou automação adicional.
___
## Arquivo de exemplo

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)

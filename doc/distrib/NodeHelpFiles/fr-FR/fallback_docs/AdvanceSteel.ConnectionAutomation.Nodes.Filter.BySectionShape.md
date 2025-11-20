## Description approfondie
Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon la forme de coupe sélectionnée.

Lors de la sélection d'une série de données structurelles avec plusieurs noeuds d'assemblage, cet exemple filtre la liste des noeuds d'assemblage selon ceux qui correspondent à la forme de coupe sélectionnée. Les éléments de données de structure peuvent comprendre des sous-listes d'éléments, de sorte que l'index est défini sur 0 (la valeur par défaut).

Dans l'exemple, un groupe d'ailes parallèles en forme de I et un HSS sont sélectionnés, et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur acceptée est le noeud d'assemblage filtré selon la forme de coupe sélectionnée.  Dans ce cas, la forme de la coupe est un 'HSS'.
___
## Exemple de fichier

![Filter.BySectionShape](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySectionShape_img.jpg)

## Description approfondie
Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon la forme de coupe sélectionnée.

Lors de la sélection d'un groupe d'assemblages composé d'une forme en W et d'un HSS, cet exemple filtre la liste des noeuds d'assemblage selon ceux qui ont une coupe aile-âme. Les éléments de données de structure peuvent avoir des sous-listes d’éléments, de sorte que l'index est défini sur 0 (la valeur par défaut).

Dans l'exemple, une forme W et un HSS sont sélectionnés, et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur acceptée est le noeud d'assemblage filtré selon la forme de coupe sélectionnée.  Dans ce cas, la forme de coupe est 'W24X104'.
___
## Exemple de fichier

![Filter.IsItAWebFlangeSection](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.IsItAWebFlangeSection_img.jpg)

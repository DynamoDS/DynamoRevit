## Description approfondie
Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon le profil de coupe sélectionné.

Lors de la sélection d'une série d'éléments de données structurelles avec plusieurs noeuds d'assemblage, filtre la liste des noeuds d'assemblage selon le profil de coupe sélectionné. Les éléments de données de structure peuvent comprendre des sous-listes d'éléments, de sorte que l'index est défini sur 0 (la valeur par défaut).

Dans l'exemple, un groupe de formes en W est sélectionné et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur de "accepted" est le noeud d'assemblage filtré selon le profil de coupe sélectionné.  Dans ce cas, le profil de coupe est 'W Shapes-Column'.
___
## Exemple de fichier

![Filter.BySectionProfile](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySectionProfile_img.jpg)

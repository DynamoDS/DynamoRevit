## Description approfondie

Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon le type de zone sélectionné: end ou body.

Lors de la sélection d'une série de données structurelles avec plusieurs noeuds d'assemblage, cet exemple filtre la liste des noeuds d'assemblage selon le profil de coupe sélectionné. Les éléments de données de structure peuvent comprendre des sous-listes d'éléments, de sorte que l'index est défini sur 0 (la valeur par défaut).

Dans l'exemple, un groupe d'ailes parallèles en forme de I est sélectionné et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur acceptée est le noeud d'assemblage filtré selon le type de zone 'zoneType' sélectionné.  Dans ce cas, le zoneType est 'end'.

___
## Exemple de fichier

![Filter.ByZoneType](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByZoneType_img.jpg)

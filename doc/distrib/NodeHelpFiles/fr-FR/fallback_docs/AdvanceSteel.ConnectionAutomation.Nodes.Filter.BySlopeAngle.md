## Description approfondie
Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon une plage de degrés min-max de l'angle d'inclinaison.  L'angle d'inclinaison est l'angle formé entre l'axe x d'un élément de données de structure et l'axe horizontal.

Dans l'exemple, un poteau en forme de W et un contreventement HSS incliné sont sélectionnés, et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur acceptée est un noeud d'assemblage dont l'angle d'inclinaison est compris entre 30 et 60 degrés. Le contreventement HSS incliné présente un angle d'inclinaison de ~60 degrés, il est donc accepté. Le poteau en forme de W présente un angle d'inclinaison de 90 degrés et est rejeté.
___
## Exemple de fichier

![Filter.BySlopeAngle](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySlopeAngle_img.jpg)

## Description approfondie
Ce noeud renvoie une liste de noeuds d'assemblage filtrés selon une plage de degrés min-max.

Dans l'exemple, un poteau en forme de W et un contreventement HSS incliné sont sélectionnés, et la sortie est constituée d'une liste de dictionnaires avec des propriétés "accepted" et "rejected".  La valeur acceptée est un noeud d'assemblage qui se trouve dans la plage de 0 à 60 degrés (ce noeud mesure à partir de l'axe X). Si la valeur maximale était de 45 degrés, le noeud d'assemblage serait rejeté.  Les paramètres 'indexFirst' et 'indexSecond' sont définis sur 'use levels' pour être alignés sur la structure de données d'entrée.
___
## Exemple de fichier

![Filter.ByAngleBetweenBeams](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenBeams_img.jpg)

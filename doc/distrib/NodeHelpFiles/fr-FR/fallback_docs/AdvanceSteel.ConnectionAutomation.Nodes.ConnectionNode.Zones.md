## Description approfondie


Ce noeud renvoie une liste de zones dans chaque noeud d'assemblage.

Dans cet exemple, la sélection d'un élément de poutre structurelle renvoie une liste de zones. L'entrée est une liste de noeuds d'assemblage, où un noeud d'assemblage regroupe des éléments de données de structure (comme des poutres et des poteaux) dans un assemblage unique.

Au sein d'un nœud d'assemblage, la zone représente une partie spécifique de l'élément de données de structure impliqué dans l'assemblage.  Les principaux types de zones sont 'extrémité' et 'corps'.

La zone 'extrémité' représente la fin d'un élément de données de structure: l'assemblage est effectué à cet endroit.

La zone 'corps' fait référence à un emplacement sur un élément de données de structure éloigné de 'extrémité, par exemple à l'endroit où un contreventement ou une ossature de barre dans l'âme d'une poutre peut s'assembler.


___
## Exemple de fichier

![ConnectionNode.Zones](./AdvanceSteel.ConnectionAutomation.Nodes.ConnectionNode.Zones_img.jpg)

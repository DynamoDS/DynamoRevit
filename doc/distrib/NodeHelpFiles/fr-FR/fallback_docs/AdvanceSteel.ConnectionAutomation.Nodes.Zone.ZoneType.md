## Description approfondie
Ce noeud récupère le type de zone pour un élément Advance Steel Zone donné.  Dans l'infrastructure d'automatisation des assemblages d'Advance Steel, les zones définissent des régions distinctes d'un élément en acier (telles que le début, le corps ou l'extrémité) où les fonctions d'assemblage peuvent être appliquées.  Le type de zone identifie la région qu'une zone particulière représente, ce qui est essentiel lors de l'automatisation du placement des joints, des coupes ou des emplacements de raidisseurs.

Dans cet exemple, plusieurs éléments sont sélectionnés, les données structurelles sont extraites, le noeud de connexion est exposé et utilisé pour identifier toutes les zones qui s'appliquent à ce noeud de connexion.  Ces zones sont l'entrée du nœud Zone.ZoneType où la sortie définit la région de l'élément en acier (début, corps ou fin).
___
## Exemple de fichier

![Zone.ZoneType](./AdvanceSteel.ConnectionAutomation.Nodes.Zone.ZoneType_img.jpg)

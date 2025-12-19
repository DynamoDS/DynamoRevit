## Description approfondie
Ce nœud étiquette les éléments Revit ayant reçu une vue, un élément, un décalage, un alignement horizontal, verticalAlignement, horizontal (si non, l'étiquette sera orientée en fonction de l'élément) et addLeader en tant qu'entrées.

Dans cet exemple, une porte est sélectionnée dans la vue "Studio Live Work Core B" et est utilisée comme entrée pour Tag.ByelementAndOffset.  L'emplacement de cette porte est extrait et utilisé comme point de départ vectoriel.  Le même point est modifié à l'aide d'un curseur changeant les points x et y et utilisé comme point d'extrémité vectoriel.  Ce vecteur est utilisé comme entrée pour le décalage avec les valeurs vraies dans les entrées horizontal et addLeader.  L'alignement horizontal est défini par les valeurs déroulantes du nœud Alignement horizontal du texte de la sélection (Gauche, Centre, Droite) et les valeurs déroulantes du nœud Alignement vertical du texte de la sélection (Bas, Milieu, Haut).

___
## Exemple de fichier

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

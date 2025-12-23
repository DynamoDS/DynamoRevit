## Description approfondie
Ce noeud prend une balise et modifie l'emplacement de sa tête.  Cela nous donne la possibilité d'automatiser un comportement de placement cohérent afin que les étiquettes soient directement au-dessus de l'élément qu'elles balisent.

Dans cet exemple, une porte est sélectionnée dans la vue "Studio Live Work Core B".  L'emplacement de cette porte est extrait, puis utilisé comme entrée d'origine pour Tag.ByElementAndLocation, avec les valeurs booléennes pour horizontal et addLeader.  L'emplacement d'origine est modifié de sorte que l'emplacement de la balise ne se superpose pas directement sur l'élément à l'aide du nœud Tag.SetHeadLocation.

___
## Exemple de fichier

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

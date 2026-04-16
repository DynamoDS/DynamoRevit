## Description approfondie
Ce noeud étiquette les éléments Revit ayant une vue, un élément, un emplacement, une vue horizontale (si non, l'étiquette sera orientée en fonction de l'élément) et addLeader en tant qu'entrées.

Dans cet exemple, une porte est sélectionnée dans la vue "Studio Live Work Core B".  L'emplacement de cette porte est extrait, puis utilisé comme entrée d'origine pour Tag.ByElementAndLocation, avec les valeurs booléennes pour horizontal et addLeader.  L'emplacement d'origine est modifié de sorte que l'emplacement de la balise ne se superpose pas directement sur l'élément à l'aide du nœud Tag.SetHeadLocation.

___
## Exemple de fichier

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

## Description approfondie
Selon un élément donné prenant en charge l'hébergement d'éléments (p. ex. des murs), « Element.GetHostedElements » renvoie les éléments qui dépendent de l'élément donné. Par défaut, les occurrences de familles qui sont hébergées par l'élément sont renvoyées. « Element.GetHostedElements » offre la possibilité d'inclure d'autres types d'éléments hébergés.

Ces éléments comprennent les suivants :
- ouvertures
- éléments hébergés dans des hôtes joints
- murs encastrés (c'est-à-dire des murs-rideaux)
- inserts intégrés partagés

Dans l'exemple ci-dessous, tous les éléments de mur sont collectés à partir de L2. Les éléments de mur sont ensuite vérifiés pour y détecter les éléments hébergés avec « Element.GetHostedElements ». Cette liste est ensuite utilisée pour créer deux listes : des murs avec des portes et des murs sans portes.
___
## Exemple de fichier

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

## Description approfondie
`Element.GetMaterials` renvoie tous les matériaux _(et leurs ID)_ qui existent dans un élément Revit. Les éléments avec plusieurs matériaux renvoient une liste pour chaque élément. L'entrée "paintMaterials" est une bascule booléenne qui spécifie au noeud de collecter également les matériaux peints par l'utilisateur.

Dans l'exemple ci-dessous, les matériaux de toutes les occurrences de mur du document actuel (fichier) sont renvoyés.
___
## Exemple de fichier

![Element.GetMaterials](./Revit.Elements.Element.GetMaterials_img.jpg)

## Description approfondie
Ce noeud récupère la matrice de transformation inverse d'un élément de lien Revit donné.  Dans Revit, les modèles liés peuvent être positionnés à l'aide de transformations (translation, rotation, mise à l'échelle). Ce nœud vous permet d'obtenir l'inverse de cette transformation, en reconvertissant les coordonnées de l'espace du modèle lié dans le système de coordonnées du modèle Revit hôte.

Dans cet exemple, tous les éléments liés Revit au niveau L3 sont sélectionnés et entrés dans LinkElement.LinkInverseTransform.  La sortie est le système de coordonnées du modèle Revit hôte.
___
## Exemple de fichier

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)

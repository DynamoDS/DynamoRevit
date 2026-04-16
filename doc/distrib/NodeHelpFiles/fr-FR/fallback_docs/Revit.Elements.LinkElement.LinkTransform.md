## Description approfondie
Ce nœud récupère la matrice de transformation appliquée à un élément de lien Revit dans le modèle hôte.
En d'autres termes, elle renvoie la transformation de position, de rotation et de mise à l'échelle qui mappe le système de coordonnées de l'élément lié dans le système de coordonnées du projet Revit hôte.
Cette option est utile lorsque vous devez aligner, analyser ou manipuler une géométrie entre des modèles liés.

Dans cet exemple, tous les éléments liés Revit au niveau L3 sont sélectionnés et entrés dans LinkElement.LinkTransform.  La sortie est la transformation de position, de rotation et de mise à l'échelle de l'élément lié.
___
## Exemple de fichier

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)

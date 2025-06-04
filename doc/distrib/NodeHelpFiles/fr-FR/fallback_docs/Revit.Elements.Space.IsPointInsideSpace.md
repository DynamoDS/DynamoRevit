## Description approfondie
Space.IsPointInsideSpace vérifie si un point donné se trouve à l'intérieur d'un espace donné. Cela peut être utile lors de l'affectation de valeurs de repère à des éléments dans Revit.

Dans l'exemple ci-dessous, toutes les bouches d'aération de la vue donnée dans le document Revit actif sont collectées. L'emplacement de ces points est ensuite comparé aux espaces de la vue donnée avec Space.IsPointInsideSpace. À l'aide de la gestion des listes, des sous-listes sont développées pour filtrer les bouches d'aération qui se trouvent dans les espaces. Pour plus d'informations sur l'utilisation de List@Level, veuillez consulter cet [article](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level).
___
## Exemple de fichier

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)

## Description approfondie
Ce noeud projette un rayon dans un modèle Revit lié à partir d'une origine et d'une direction spécifiées, puis trace ses rebonds successifs sur les éléments liés. Chaque rebond représente un point où le rayon croise la géométrie dans le modèle lié, jusqu'à un nombre maximal défini de réflexions.

Dans cet exemple, un élément lié est sélectionné et l'emplacement de cet élément est utilisé comme entrée d'origine de LinkElement.ByRayBounce avec une direction, maxBounces et une vue.  Les extrants sont des points et des éléments liés.
___
## Exemple de fichier

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)

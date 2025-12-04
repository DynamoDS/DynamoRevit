## Description approfondie
Ce nœud effectue une analyse des rebonds de rayon dans le modèle Revit. En partant d'un point d'origine donné et en se déplaçant le long d'un vecteur de direction spécifié, il trace la trajectoire du rayon lorsqu'il croise des éléments du modèle. Lorsque le rayon frappe une surface, il peut continuer à rebondir sur cette surface, en fonction du nombre de rebonds autorisés, simulant ainsi la lumière, la visibilité ou le comportement de réflexion de la trajectoire.

Dans cet exemple, un élément est sélectionné et son emplacement est utilisé pour l'origine d'entrée du nœud RayBounce.ByOriginDirection, ainsi qu'une direction, maxBounces et une vue.
___
## Exemple de fichier

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)

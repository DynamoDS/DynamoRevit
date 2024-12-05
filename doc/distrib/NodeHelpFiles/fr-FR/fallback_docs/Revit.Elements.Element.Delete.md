## Description approfondie
« Element.Delete » fonctionne de la même façon que l'option de suppression dans l'interface Revit. Elle supprime l'élément d'entrée et tous les éléments qui en dépendent.

Par exemple, la suppression d'un mur contenant des portes supprime également les portes. La sortie consiste en une liste imbriquée des ID des éléments qui ont été supprimés. Remarque : il est recommandé d'utiliser ce nœud en mode d'exécution manuelle dans Dynamo.

Dans l'exemple ci-dessous, toutes les occurrences de la famille « Bouton d'aide » sont supprimées du document actuel (fichier).
___
## Exemple de fichier

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

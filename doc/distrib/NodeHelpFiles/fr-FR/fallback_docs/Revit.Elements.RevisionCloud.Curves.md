## Description approfondie
Ce nœud extrait les boucles de courbes (généralement des arcs et des lignes) qui constituent le périmètre visible d'un nuage de révision. Chaque segment du nuage est représenté par un objet courbe (généralement un arc) correspondant à la forme "bulle" du repère de révision dans une vue ou une feuille.

Dans cet exemple, un rectangle est créé à l'aide de curseurs numériques pour définir ses cotes, qui sont ensuite décomposées en courbes et inversées pour s'orienter. Ces courbes, ainsi qu'une vue sélectionnée (Plan du site) et une révision (Séq. 2 – Non destiné à la construction), sont utilisées pour générer un nuage de révision avec le nœud RevisionCloud.ByCurve. Le nuage de révision créé est ensuite connecté au nœud RevisionCloud.Curves, qui extrait et affiche les courbes définies de ce nuage. Cela aide les utilisateurs à vérifier la géométrie du nuage de révision et offre une flexibilité pour la réutilisation ou une automatisation plus poussée.
___
## Exemple de fichier

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)

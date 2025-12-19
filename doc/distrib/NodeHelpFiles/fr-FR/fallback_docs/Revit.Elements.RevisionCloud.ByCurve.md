## Description approfondie
Ce nœud crée un nuage de révision dans une vue spécifiée.  Les entrées sont la vue , la liste des courbes "contour du nuage" et une révision définie.

Dans cet exemple, deux curseurs numériques définissent la largeur et la longueur du rectangle, qui est ensuite décomposé en courbes. Ces courbes sont inversées pour conserver une orientation correcte, puis connectées au nœud RevisionCloud.ByCurve. Le graphique prend également en compte la vue active (plan du premier étage) et la révision choisie (Séq. 2 – Non destiné à la construction) en entrée. Ensemble, ils génèrent automatiquement un nuage de révision dans la vue sélectionnée en fonction de la forme définie.
___
## Exemple de fichier

![RevisionCloud.ByCurve](./Revit.Elements.RevisionCloud.ByCurve_img.jpg)

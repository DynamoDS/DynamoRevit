## Description approfondie
PathOfTravel.LongestOfShortestExitPaths crée une trajectoire de déplacement représentant le plus long des plus courts itinéraires entre un groupe de points et un ou plusieurs points de sortie. Chaque point de départ est évalué par rapport aux sorties disponibles, et Revit détermine la trajectoire de sortie valide la plus proche pour chacun d'eux. Le nœud renvoie ensuite la trajectoire présentant la plus grande distance de déplacement parmi ces résultats de plus court chemin.

Dans l'exemple ci-dessous, un niveau et une vue en plan d'étage sont d'abord créés afin de fournir la vue Revit requise pour le calcul de la trajectoire de déplacement. Plusieurs points sont générés à l'aide de Point.ByCoordinates, puis combinés dans une liste représentant les emplacements de sortie possibles. PathOfTravel.LongestOfShortestExitPaths utilise la vue en plan d'étage et la liste de points de destination pour calculer les trajectoires de déplacement, et renvoie la trajectoire présentant la plus grande distance de déplacement.
___
## Exemple de fichier

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

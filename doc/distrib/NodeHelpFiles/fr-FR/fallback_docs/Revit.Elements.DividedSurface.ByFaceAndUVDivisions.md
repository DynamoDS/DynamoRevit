## Description approfondie
Ce nœud crée un nouvel élément de surface divisée sur une face sélectionnée d'un élément Revit et définit sa disposition à l'aide des divisions U et V spécifiées.  Une surface divisée est un quadrillage à motifs appliqué à une face, généralement utilisé pour positionner des panneaux de mur-rideau, des composants adaptatifs ou des systèmes de panneaux sur une surface de coffrage.

Les divisions U et V déterminent le nombre de subdivisions qui se produisent dans chaque direction de surface, tandis que le paramètre de rotation ajuste l’orientation de la grille par rapport au système de coordonnées U-V de la surface.

Dans cet exemple, une face est sélectionnée et utilisée en tant qu’entrée de surface pour le noeud DividedSurface.ByFaceUVDivisions avec les UDivs, les VDivs qui sont contrôlés par des curseurs.  Les derniers noeuds exposent les valeurs de la surface divisée.  Lors de l'exécution de cet exemple de graphique, vous devrez respecter l'avertissement Revit et supprimer les éléments suggérés afin que les grilles apparaissent sur la surface sélectionnée.

Voir le lien pour obtenir plus d'informations.
https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## Exemple de fichier

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## Exemple de fichier

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)

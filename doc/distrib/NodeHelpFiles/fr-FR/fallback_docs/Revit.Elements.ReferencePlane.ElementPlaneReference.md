## Description approfondie
Ce nœud extrait la référence d'élément Revit réelle d'un plan de référence sélectionné. Cette option est utile lorsque vous devez utiliser ce plan comme référence d'hébergement pour la géométrie ou les cotes dans Revit.

Exemple:
Dans ce diagramme, deux points sont définis à l'aide de coordonnées et un plan de référence est créé entre eux avec ReferencePlane.ByStartPointEndPoint. Ce plan de référence est ensuite connecté à ReferencePlane.ElementPlaneReference, qui génère la référence native Revit du plan, ce qui le rend prêt à être utilisé pour des tâches d'hébergement ou d'alignement.
___
## Exemple de fichier

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

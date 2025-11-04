## Description approfondie
Placez une FamilyInstance (instance de famille) Revit dans un projet Revit, en fonction du FamilyType (type de famille) souhaité et de son système de coordonnées.

Dans cet exemple, un type de famille et un point du système de coordonnées spécifiques sont fournis et une instance de famille est placée.
Un cas d’utilisation courant consiste à créer un système de coordonnées basé sur le point de base du projet Revit, puis à le faire pivoter pour correspondre à la rotation du site. Vous pouvez ensuite introduire ce système de coordonnées transformé dans le nœud FamilyInstance.ByCoordinateSystem pour placer les instances de la famille à leurs emplacements réels, en tenant compte de l’orientation du site.

Pour en savoir plus sur les systèmes de coordonnées, consultez le lien ci-dessous.
https://help.autodesk.com/view/RVT/2025/ENU/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Exemple de fichier

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

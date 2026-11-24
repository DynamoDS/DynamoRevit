## Description approfondie
ElementCurveReference.ByCurve crée une référence de courbe d'élément Revit à partir d'une courbe Dynamo. Une référence de courbe est utile lorsqu'un autre nœud Revit a besoin d'une courbe sélectionnable ou référençable, plutôt que d'une simple géométrie Dynamo. Elle est couramment utilisée dans les workflows qui nécessitent des références Revit, par exemple pour créer des cotes, des alignements, des contraintes ou d'autres éléments devant référencer la géométrie du modèle.

Dans l'exemple ci-dessous, une courbe est sélectionnée et utilisée comme entrée pour ElementCurveReference.ByCurve. La sortie est une ElementCurveReference, qui est ensuite utilisée comme référence de courbe Revit pour les nœuds en aval nécessitant des références basées sur des courbes.
___
## Exemple de fichier

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

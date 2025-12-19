## Description approfondie
Ce noeud récupère la référence de courbe associée à un élément de courbe Revit donné, telle qu'une courbe de modèle ou une ligne de détail. La référence peut ensuite être utilisée comme entrée pour d'autres nœuds qui nécessitent une référence de géométrie, comme la cotation, l'alignement ou la création de chemins divisés.

Dans cet exemple, une courbe de modèle est créée à l'aide d'un point de départ et d'un point d'arrivée, puis entrée dans le nœud CurveElement.ElementCurveReference.  La sortie est une référence géométrique de l'élément courbe qui peut être utilisée dans les opérations en aval.
___
## Exemple de fichier

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

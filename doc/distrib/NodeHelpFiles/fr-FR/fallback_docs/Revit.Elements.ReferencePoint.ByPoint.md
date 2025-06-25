## Description approfondie
'ReferencePoint.ByPoint' crée un élément de point de référence dans le document de famille Revit actif à l'emplacement de point donné. Remarque: le document de famille doit être un composant adaptatif ou une famille de volumes.Ce noeud diffère de 'ReferencePoint.ByCoordinates' en ce qu'il utilise un point Dynamo pour l'emplacement. Cette option est utile, car l'utilisateur final peut utiliser la manipulation directe pour modifier le point Dynamo, ce qui entraîne également la mise à jour du point de référence. Pour en savoir plus sur la manipulation directe, visitez ce [lien](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

Dans l'exemple ci-dessous, un nouveau point de référence est créé aux coordonnées 0,0,1.
___
## Exemple de fichier

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

## Description approfondie
Ce noeud place des composants adaptatifs en appliquant des valeurs de paramètres UV à une face sélectionnée, définissant ainsi les emplacements de placement pour le type de famille adaptative.

Dans cet exemple, une surface est créée dans la famille de volumes en extrudant une courbe (ce qui est fait manuellement) et cette surface est sélectionnée comme entrée de face. Les valeurs UV sont ensuite fournies pour déterminer les positions de placement, et la famille Trépied de diagnostic – 1 Point.rfa est utilisée comme type. Le nœud AdaptiveComponent.ByParametersOnFace génère des composants adaptatifs positionnés sur la face sélectionnée.  Notez que le « Trépied de diagnostic – 1 Point.rfa » doit être chargé dans votre famille de volumes avant d’exécuter ce graphique.

Pour exécuter ce fichier d'exemple d'aide sur le nœud, vous devez charger "Diagnostics Tripod-1 point.rfa" dans le fichier Revit. La famille est stockée ici. C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## Exemple de fichier

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)

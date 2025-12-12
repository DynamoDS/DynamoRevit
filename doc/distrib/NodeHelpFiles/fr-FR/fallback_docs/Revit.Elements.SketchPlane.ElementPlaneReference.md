## Description approfondie
Ce noeud récupère le plan de référence associé à un élément de plan d'esquisse donné. Cela permet d’identifier ou de réutiliser le même plan de référence pour créer ou modifier la géométrie.

Dans cet exemple, un plan est défini, puis connecté au nœud SketchPlane.ByPlane, qui génère un plan d'esquisse correspondant.  Ce plan d'esquisse est utilisé en tant qu'entrée de SketchPlane.ElementPlaneReference, où la sortie peut ensuite être utilisée pour la cotation, l'alignement, les contraintes ou d'autres opérations nécessitant une référence Revit.

___
## Exemple de fichier

![SketchPlane.ElementPlaneReference](./Revit.Elements.SketchPlane.ElementPlaneReference_img.jpg)

## Description approfondie

Renvoie le type d’unité d’un paramètre.

Dans l’exemple ci-dessous, nous extrayons tous les paramètres des éléments et les utilisons en entrée. L’objectif est d’afficher le type d’unité de chaque paramètre.
Par exemple, si Element.Parameters affiche « Diamètre de base : 1' – 3 1/4” », la sortie de Parameter.Unit sera « Unité (Nom = Pieds ou Mètres) ».
Si un paramètre n’a pas d’unité (comme Element.Parameters = Apply Cut : No), alors Parameter.Unit retournera null.
Étant donné que Dynamo lui-même est sans unité, il est essentiel d’identifier les types d’unités entrantes pour effectuer des calculs précis. Ce nœud permet à Dynamo de reconnaître et de traiter ces données unitaires.

___
## Exemple de fichier

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

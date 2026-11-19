## Description approfondie
Gate contrôle si les données d'entrée sont autorisées à passer, en fonction d'une valeur booléenne. Si la porte est ouverte (True), l'entrée est transmise à la sortie. Si la porte est fermée (False), l'entrée est bloquée.

Dans l'exemple ci-dessous, un élément Gate est utilisé pour empêcher la géométrie d'être décomposée à chaque exécution du graphe. Lorsque la porte est ouverte (True), la géométrie nouvellement créée passe et est décomposée. Lorsque la porte est fermée (False), la géométrie est bloquée et n'est pas transmise ni décomposée à nouveau.
___
## Exemple de fichier

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)

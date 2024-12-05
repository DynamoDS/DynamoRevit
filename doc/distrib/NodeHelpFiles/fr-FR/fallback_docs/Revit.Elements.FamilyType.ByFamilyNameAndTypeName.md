## Description approfondie
Semblable à 'Revit.Elements.FamilyType.ByFamilyAndName', 'Revit.Elements.FamilyType.ByFamilyNameAndTypeName' renvoie la définition du type de famille à partir du document actif (si disponible). Ce comportement ressemble à celui de 'Revit.Elements.FamilyType.ByFamilyAndName', mais au lieu d'utiliser une définition de famille, ce noeud s'appuie sur une saisie de chaîne pour les deux valeurs. Si le type de famille n'est pas disponible dans le document actif, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, un type de famille de portes, 36" x 84", de la famille "Door-Passage-Single-Flush" est renvoyé.
___
## Exemple de fichier

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)

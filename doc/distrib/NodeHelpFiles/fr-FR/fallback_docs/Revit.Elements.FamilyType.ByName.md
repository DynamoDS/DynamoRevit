## Description approfondie
'FamilyType.ByName' tente de récupérer le type de famille donné du nom donné à partir du document actif. Si le type de famille n'est pas disponible dans le document actif, une valeur nulle est renvoyée.

Remarque: 'FamilyType.ByName' recherche les définitions de type de famille dans l'ordre de création de la famille parent (par ID d'élément). Si plusieurs familles parents ont une définition de type portant le même nom, la première trouvée est renvoyée. Pour une recherche plus concise des types de familles, utilisez 'FamilyType.ByFamilyAndName' ou 'FamilyType.ByFamilyNameAndTypeName'.

Dans l'exemple ci-dessous, un type de famille de portes, 36" x 84", de la famille "Door-Passage-Single-Flush" est renvoyé.
___
## Exemple de fichier

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)

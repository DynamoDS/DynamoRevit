## Description approfondie
Ce nœud prend une valeur de longueur numérique et un identificateur de type d'unité, convertissant la valeur en entrée en unités de longueur du projet Revit actif. La sortie est une valeur double représentant le résultat converti.

Dans cet exemple, un curseur numérique fournit une valeur de longueur et une unité (par exemple, Mètres) est sélectionnée pour obtenir sa chaîne Unit.TypeId. Les deux sont connectés au nœud UnitsUtilities.ConvertToCurrentProjectLengthUnit, qui renvoie la valeur de longueur convertie en fonction des paramètres d'unité du projet.
___
## Exemple de fichier

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)

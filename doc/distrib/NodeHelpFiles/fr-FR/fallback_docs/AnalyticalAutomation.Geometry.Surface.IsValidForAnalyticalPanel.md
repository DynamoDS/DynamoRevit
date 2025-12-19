## Description approfondie
Ce noeud évalue une surface donnée pour déterminer si elle peut être utilisée dans la définition d'un panneau analytique. Une surface valide est généralement plane, continue et peut être convertie en représentation analytique dans l'environnement de modèle analytique de Revit.

Dans cet exemple, les faces d'un élément de dalle du projet sont collectées et la face supérieure est fournie au noeud en entrée. Le noeud renvoie un résultat booléen indiquant si la surface sélectionnée répond aux exigences de création d'un panneau analytique, ainsi qu'un message facultatif décrivant les problèmes rencontrés lors de la validation.
___
## Exemple de fichier

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

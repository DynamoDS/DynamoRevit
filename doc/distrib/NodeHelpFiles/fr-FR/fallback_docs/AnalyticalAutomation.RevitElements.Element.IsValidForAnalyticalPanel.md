## Description approfondie
Ce nœud évalue l'élément sélectionné pour déterminer s'il est adapté à la génération d'un panneau analytique. L'entrée facultative checkOpenings spécifie si les ouvertures au sein de l'élément doivent être incluses dans le contrôle de validité. Lorsqu'elles sont définies sur true, les ouvertures sont prises en compte dans le cadre de l'évaluation.

Dans cet exemple, l'élément est défini par son ID d'élément à l'aide du nœud Element.ById et fourni à Element.IsValidForAnalyticalPanel. Les sorties incluent une valeur booléenne indiquant si l'élément est valide et un message d'exception décrivant les problèmes empêchant son utilisation.
___
## Exemple de fichier

![Element.IsValidForAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.IsValidForAnalyticalPanel_img.jpg)

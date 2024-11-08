## Description approfondie
`ScheduleFilter.FilterType` renvoie la méthode utilisée pour le filtre d'entrée.
Les types de filtres possibles sont les suivants :

- Egal : la valeur du champ est égale à la valeur spécifiée.
- Non égal : la valeur du champ n'est pas égale à la valeur spécifiée.
- Supérieur à : la valeur du champ est supérieure à la valeur spécifiée.
- Supérieur ou égal : la valeur du champ est supérieure ou égale à la valeur spécifiée.
- Inférieur à : la valeur du champ est inférieure à la valeur spécifiée.
- Inférieur ou égal : la valeur du champ est inférieure ou égale à la valeur spécifiée.
- Contient : pour un champ de chaîne, la valeur du champ contient la chaîne spécifiée.
- Ne contient pas : pour un champ de chaîne, la valeur du champ ne contient pas la chaîne spécifiée.
- Commence par : pour un champ de chaîne, la valeur du champ commence par la chaîne spécifiée.
- Ne commence pas par : pour un champ de chaîne, la valeur du champ ne commence pas par la chaîne spécifiée.
- Termine par : pour un champ de chaîne, la valeur du champ se termine par la chaîne spécifiée.
- Ne termine pas par : pour un champ de chaîne, la valeur du champ ne se termine pas par la chaîne spécifiée.
- Est associé au paramètre global  : le champ est associé à un paramètre global spécifié de type compatible
- N'est pas associé à un paramètre global : le champ n'est pas associé à un paramètre global spécifié d'un type compatible

Dans l'exemple ci-dessous, la première nomenclature du fichier Revit actuel est collectée. La vue de nomenclature est ensuite recherchée pour les filtres et le seul filtre appliqué est un type de filtre "la chaîne ne se termine pas par".
___
## Exemple de fichier

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

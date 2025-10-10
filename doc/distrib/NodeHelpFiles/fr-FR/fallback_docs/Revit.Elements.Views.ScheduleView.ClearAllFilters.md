## Description approfondie
Supprime tous les filtres définis d’une entrée donnée d’un ScheduleView.

Dans cet exemple, nous avons d’abord défini une vue de nomenclature acheminée directement vers ScheduleView.ScheduleFilters afin d’afficher explicitement ses filtres de vue préexistants. Pour mettre en évidence la transformation, cette même vue passe ensuite par une brève pause de 10 millisecondes. Après ce court délai, la vue est transmise au nœud ScheduleView.ClearAllFilters, qui supprime tous les filtres appliqués. Enfin, la vue ainsi modifiée est renvoyée vers un autre nœud ScheduleView.ScheduleFilters, démontrant sans équivoque que sa liste de filtres est devenue une liste vide, confirmant ainsi la suppression réussie de tous les filtres d’origine.
___
## Exemple de fichier

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

## Description approfondie
Supprime tous les filtres définis d’une entrée donnée d’un ScheduleView.

Dans cet exemple, nous avons initialement défini une vue de planification qui est acheminée directement vers ScheduleView.ScheduleFilters pour afficher explicitement ses filtres de vue préexistants. Pour mettre en évidence la transformation, cette même vue passe ensuite par une brève pause de 10 millisecondes. Après ce court délai, la vue passe au nœud ScheduleView.ClearAllFilters, qui supprime tous les filtres appliqués. Enfin, la vue désormais modifiée est recanalisée dans un autre nœud ScheduleView.ScheduleFilters, démontrant sans équivoque que sa liste de filtres est devenue une liste vide, et confirmant ainsi l’effacement réussi de tous les filtres d’origine.
___
## Exemple de fichier

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

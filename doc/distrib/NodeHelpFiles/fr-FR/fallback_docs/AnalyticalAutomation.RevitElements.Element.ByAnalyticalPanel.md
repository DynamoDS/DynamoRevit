## Description approfondie
Ce nœud crée un élément physique de mur ou de sol à partir d'un panneau analytique, ou met à jour un élément physique existant qui est déjà connecté au panneau analytique. L'élément physique peut hériter de la géométrie, des paramètres et des ouvertures du panneau analytique, en fonction des paramètres d'entrée. Lorsque le panneau analytique n'a pas d'élément physique associé, le nœud en crée un nouveau basé sur le modèle analytique.

Dans cet exemple, un panneau de tablier de toiture analytique du modèle Supermarché ACO est sélectionné. Ce panneau analytique est connecté à ce noeud, et les valeurs booléennes en entrée illustrent le comportement par défaut pour la mise à jour de la géométrie, des paramètres et l'inclusion des ouvertures. Le noeud crée ensuite, ou met à jour, l'élément de toiture physique à partir de ce panneau analytique.
___
## Exemple de fichier

![Element.ByAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.ByAnalyticalPanel_img.jpg)

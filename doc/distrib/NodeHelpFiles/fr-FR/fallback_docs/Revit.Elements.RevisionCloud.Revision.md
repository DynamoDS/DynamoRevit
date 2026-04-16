## Description approfondie
Ce nœud extrait l'élément de révision lié à un nuage de révision spécifique dans Revit. Il fournit les données de révision associées à ce nuage, en permettant aux utilisateurs de vérifier, de suivre ou de valider les détails de révision par programmation dans leur projet.

Dans cet exemple, un rectangle est créé avec des curseurs numériques pour la largeur et la longueur, puis décomposé en courbes et inversé pour une orientation appropriée. Ces courbes, ainsi qu'une vue choisie (L1_SD) et une révision sélectionnée (Séq. 2 – Non destiné à la construction), sont utilisées pour générer un nuage de révision via le nœud RevisionCloud.ByCurve. Le nuage de révision obtenu est connecté au nœud RevisionCloud.Revision, qui récupère et génère la révision associée à ce nuage. Cela permet de s'assurer que les utilisateurs peuvent confirmer quelle révision est liée à chaque nuage de révision.
___
## Exemple de fichier

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)

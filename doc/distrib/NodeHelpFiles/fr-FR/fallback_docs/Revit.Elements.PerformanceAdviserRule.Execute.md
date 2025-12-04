## Description approfondie
Ce nœud exécute une règle Performance Adviser spécifique sur un ensemble d'éléments Revit.

Dans cet exemple, la règle Performance Adviser vérifie si l'option "Découpage de la vue est désactivée". Les résultats sont transmis au nœud FailureMessage.FailingElements, qui génère les éléments spécifiques du modèle qui a échoué à cette vérification. Ce processus permet aux utilisateurs de retracer et de corriger plus facilement les éléments exacts responsables des problèmes.

___
## Exemple de fichier

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)

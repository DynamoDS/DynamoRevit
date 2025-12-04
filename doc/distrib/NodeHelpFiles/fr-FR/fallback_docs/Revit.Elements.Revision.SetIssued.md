## Description approfondie
Le nœud Revision.SetIssued de Dynamo vous permet de contrôler si une révision sélectionnée dans Revit est marquée comme émise ou non. Il prend un élément de révision et une entrée booléenne (Vrai/Faux), ce qui donne aux utilisateurs un contrôle direct sur l'état de la révision sans avoir à la modifier manuellement dans Revit.
Dans ce graphique, le noeud Sélectionner une révision est utilisé pour sélectionner une révision en particulier (p. ex., "Séqu. 1 – Conception schématique"). Le nœud booléen fournit une valeur Vrai/Faux, qui est ensuite connectée au nœud Revision.SetIssued pour mettre à jour automatiquement l'état émis de la révision.

___
## Exemple de fichier

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

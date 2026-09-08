## Description approfondie
GroupType.ByTypeId est principalement utilisé dans les workflows de création et de gestion de paramètres lorsqu'un groupe de paramètres Revit doit être spécifié par programmation. L'entrée est une chaîne représentant un ID de type de groupe de paramètres Revit. La sortie est un objet GroupType qui peut être utilisé en aval dans les nœuds nécessitant une classification de groupe de paramètres.

Dans l'exemple ci-dessous, plusieurs chaînes représentant des ID de type de groupe de paramètres Revit sont créées et combinées dans une liste. La liste est ensuite utilisée comme entrée pour GroupType.ByTypeId. La sortie est une liste d'objets GroupType Revit représentant les groupes de paramètres correspondants.
___
## Exemple de fichier

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)

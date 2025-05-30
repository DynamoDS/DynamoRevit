## Description approfondie
'Transaction.End' termine la transaction Dynamo en cours et renvoie l'élément spécifié. Dans Revit, les transactions représentent les modifications apportées au document Revit. L'activation du bouton Annuler indique qu'une modification a été apportée. À l'aide de 'Transaction.End', les utilisateurs peuvent ajouter des étapes au graphique Dynamo, créant ainsi une action d'annulation pour chaque étape où 'Transaction.End' est utilisé.

Dans l'exemple ci-dessous, une occurrence de famille est placée dans le document Revit. 'Transaction.End' est appelé pour terminer le placement avant de faire pivoter l'occurrence de famille avec 'FamilyInstance.SetRotation'.

___
## Exemple de fichier

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)

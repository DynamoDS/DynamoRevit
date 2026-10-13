## Description approfondie
Transaction.Start démarre une transaction dans le document Revit actif. Les transactions servent à regrouper les modifications apportées au modèle Revit afin qu'elles puissent être validées ou annulées ensemble. Transaction.Start est généralement utilisé avec Transaction.End lorsqu'un workflow Dynamo nécessite un contrôle explicite du début et de la fin des modifications du document Revit.

Dans l'exemple ci-dessous, un niveau est créé et utilisé comme entrée pour Transaction.Start. Cela démarre une transaction Revit avant que les modifications du modèle soient effectuées. Une fois les opérations Revit requises terminées, Transaction.End ferme la transaction et valide les modifications dans le document. La sortie est un objet de transaction qui peut être transmis à d'autres nœuds liés aux transactions.
___
## Exemple de fichier

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)

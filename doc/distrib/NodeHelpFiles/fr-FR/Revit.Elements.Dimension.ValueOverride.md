## Description approfondie
`Dimension.ValueOverride` renvoie le remplacement de valeur de la dimension donnée, si elle a une valeur remplacée. Pour les dimensions à plusieurs segments, une liste imbriquée de valeurs est renvoyée. Si la dimension n'a pas de valeur remplacée, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, la première cote est collectée dans la vue active et vérifiée pour savoir si elle comporte une cote remplacée. Si la cote est remplacée, elle est effacée avec la valeur `Dimension.SetValueOverride`.
___
## Exemple de fichier

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)

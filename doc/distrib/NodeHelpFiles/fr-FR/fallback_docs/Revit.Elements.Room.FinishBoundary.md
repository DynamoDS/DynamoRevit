## Description approfondie
'Room.FinishBoundary' renvoie une liste imbriquée représentant la limite finie de la pièce donnée. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent des boucles dans la pièce. La limite de pièce renvoyée par ce noeud est située sur la face finie à l'intérieur de la pièce Revit. Pour plus d'informations sur les lignes d'emplacement de murs, consultez cet [article d'aide](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si une pièce non délimitée ou non placée est donnée, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, toutes les pièces sont collectées à partir du document actif et de la vue sélectionnée. Les limites finies sont ensuite renvoyées.
___
## Exemple de fichier

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)

## Description approfondie
'Room.CoreCenterBoundary' renvoie une liste imbriquée représentant la limite centrale de porteur de la pièce donnée. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent des boucles dans la pièce. Les limites centrales de porteur sont situées au centre des murs définis en tant que porteurs. Pour plus d'informations sur les lignes d'emplacement de murs, consultez cet [article d'aide](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si une pièce non délimitée ou non placée est donnée, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, toutes les pièces sont collectées à partir du document actif et de la vue sélectionnée. Les limites centrales de porteur sont ensuite renvoyées.
___
## Exemple de fichier

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)

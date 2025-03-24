## Description approfondie
'Room.CenterBoundary' renvoie une liste imbriquée représentant la limite d'axe de la pièce données. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent des boucles dans les pièces. Les limites centrales sont situées sur l'axe du mur à travers toutes les couches dans la pièce Revit. Pour plus d'informations sur les lignes d'emplacement de murs, consultez cet [article d'aide](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si une pièce non délimitée ou non placée est donnée, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, toutes les pièces sont collectées à partir du document actif et de la vue sélectionnée. Les limites centrales sont ensuite renvoyées.
___
## Exemple de fichier

![Room.CenterBoundary](./Revit.Elements.Room.CenterBoundary_img.jpg)

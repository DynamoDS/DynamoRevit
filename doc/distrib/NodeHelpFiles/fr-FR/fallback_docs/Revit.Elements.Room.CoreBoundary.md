## Description approfondie
'Room.CoreBoundary' renvoie une liste imbriquée représentant la limite de porteur de la pièce donnée. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent des boucles dans les pièces. Les limites de porteur sont situées sur la couche intérieure ou extérieure du porteur le plus proche de la pièce. Pour plus d'informations sur les lignes d'emplacement de murs, consultez cet [article d'aide](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si une pièce non délimitée ou non placée est donnée, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, toutes les pièces sont collectées à partir du document actif et de la vue sélectionnée. Les limites de porteur sont ensuite renvoyées.
___
## Exemple de fichier

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)

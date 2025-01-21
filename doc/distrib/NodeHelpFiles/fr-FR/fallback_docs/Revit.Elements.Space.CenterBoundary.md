## Description approfondie
Space.CenterBoundary renvoie une liste imbriquée représentant la limite de la ligne centrale de l'espace donné. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent les boucles à l'intérieur de l'espace. Les limites du centre sont situées sur la ligne centrale du mur sur toutes les couches de l'espace Revit. Pour plus d'informations sur les lignes de justification des murs, consultez cet [article](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si un espace non délimité ou non placé est indiqué, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, tous les espaces sont collectés à partir du document actif et de la vue sélectionnée. Les limites de centre sont ensuite renvoyées.
___
## Exemple de fichier

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)

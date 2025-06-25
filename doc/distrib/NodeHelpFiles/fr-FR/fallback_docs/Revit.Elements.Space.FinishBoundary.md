## Description approfondie
Space.FinishBoundary renvoie une liste imbriquée représentant la limite de finition de l'espace donné. Dans la liste renvoyée, la première sous-liste représente les courbes les plus à l'extérieur, tandis que les listes suivantes représentent les boucles à l'intérieur de l'espace. La limite d'espace renvoyée par ce nœud est située sur la face de finition à l'intérieur de l'espace Revit. Pour plus d'informations sur les lignes de justification des murs, consultez cet [article](https://help.autodesk.com/view/RVT/2024/FRA/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89).

Si un espace non délimité ou non placé est indiqué, une valeur nulle est renvoyée.

Dans l'exemple ci-dessous, tous les espaces sont collectés à partir du document actif et de la vue sélectionnée. Les limites de finition sont ensuite renvoyérs.

___
## Exemple de fichier

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)

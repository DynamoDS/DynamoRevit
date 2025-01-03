## Description approfondie
'Element.Solids' renvoie la géométrie solide de l'élément donné. Les solides sont renvoyés sous forme de listes imbriquées, car un élément donné peut avoir plus d'un solide en son sein. Si un seul solide représentant l'élément est souhaité, 'Solid.ByUnion' peut être utilisé dans les listes.

Dans l'exemple ci-dessous, tous les murs sont collectés à partir de la vue sélectionnée. Les murs qui ont été créés en tant que familles in situ sont ensuite supprimés et les solides des murs restants sont renvoyés.

___
## Exemple de fichier

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

## Description approfondie
'Room.IsInsideRoom' renvoie une valeur booléenne indiquant si le point donné est présent ou non dans la pièce donnée.

Dans l'exemple ci-dessous, tous les meubles du document Revit actif sont collectés, ainsi que toutes les pièces. Les points d'emplacement du mobilier sont ensuite transmis dans 'Room.IsInsideRoom' pour vérifier dans quelle pièce se trouvent les points donnés (le cas échéant). Enfin, le mobilier est filtré selon les éléments disposant d'emplacements de pièce, et ces valeurs sont combinées dans des listes.
___
## Exemple de fichier

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

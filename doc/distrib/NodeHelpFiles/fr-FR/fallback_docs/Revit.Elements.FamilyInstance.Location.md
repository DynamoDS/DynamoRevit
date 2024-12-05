## Description approfondie
'FamilyInstance.Location' renvoie un point Dynamo pour l'occurrence de famille donnée. En cas d'absence d'emplacement, une valeur nulle est renvoyée. 'FamilyInstance.Location' fonctionne sur un élément basé sur des points et ne renvoie pas d'emplacement pour un élément basé sur une courbe dans Revit, _par exemple un élément de mur ou de poutre_.

Dans l'exemple ci-dessous, toutes les occurrences de famille de portes de la vue active du document actif sont collectées. Les emplacements des portes sont ensuite renvoyés avec 'FamilyInstance.Location'.

Dans le cas de cet exemple, les portes de murs-rideaux renvoient une valeur nulle. Les emplacements des panneaux de murs-rideaux sont disponibles via les nœuds de panneaux de murs-rideaux.
___
## Exemple de fichier

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)

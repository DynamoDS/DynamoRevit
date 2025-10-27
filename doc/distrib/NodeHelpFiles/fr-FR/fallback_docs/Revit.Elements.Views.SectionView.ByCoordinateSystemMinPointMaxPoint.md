## Description approfondie
Crée des vues en coupe à partir de n’importe quel système de coordonnées avec des points Min et Max.

Dans cet exemple, nous générons une SectionView dont le placement et l’orientation sont directement corrélés au CoordinateSystem en entrée. Ici, ce système de coordonnées a été stratégiquement positionné pour définir une coupe axée sur l'angle supérieur droit du bâtiment. Il est essentiel d’observer que la composante Z de notre entrée minPoint et maxPoint dicte avec précision le paramètre Far Clip Offset (décalage de délimitation éloignée) de la vue en coupe Revit résultante, contrôlant ainsi sa profondeur de visualisation effective dans le modèle.
Pour en savoir plus sur l’affichage des éléments de contrôle, consultez le lien ci-dessous.
https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## Exemple de fichier

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)

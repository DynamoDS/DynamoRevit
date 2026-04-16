## Description approfondie
Ce nœud crée une vue d'altitude à partir d'un marqueur d'altitude existant en spécifiant l'index du marqueur. Chaque marqueur d'altitude dans Revit peut héberger jusqu'à quatre vues d'altitude individuelles, une pour chaque direction (nord, sud, est et ouest). Ce nœud vous permet de générer l'une de ces altitudes directionnelles en référençant le marqueur et le numéro d'index souhaité.

Dans cet exemple, un marqueur d'altitude est créé et utilisé comme marqueur d'altitude en entrée pour le nœud ElevationMarker.CreateElevationByMarkerIndex avec une vue et un index (0,1,2,3).

___
## Exemple de fichier

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

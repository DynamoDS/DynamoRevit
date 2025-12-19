## Im Detail
Dieser Block erstellt eine Ansicht aus einem vorhandenen ElevationMarker-Element, indem der Markierungsindex angegeben wird. Jedes ElevationMarker-Element in Revit kann bis zu vier einzelne Ansichten hosten - eine für jede Richtung (Nord, Süd, Ost und West). Mit diesem Block können Sie eine dieser Richtungsansichten erstellen, indem Sie die Markierung und die gewünschte Indexnummer referenzieren.

In diesem Beispiel wird eine Ansichtsmarkierung erstellt und als Eingabe-elevationMarker für den ElevationMarker.CreateElevationByMarkerIndex-Block zusammen mit einer Ansicht und einem Index (0,1,2,3) verwendet.

___
## Beispieldatei

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

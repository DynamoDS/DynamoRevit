## Im Detail
Platziert ein Revit-FamilyInstance-Objekt anhand des gewünschten FamilyType und des zugehörigen Koordinatensystems in einem Revit-Projekt.

In diesem Beispiel werden ein bestimmter Familientyp und ein Koordinatensystempunkt bereitgestellt und ein Familienexemplar platziert.
Ein häufiger Anwendungsfall besteht darin, ein Koordinatensystem basierend auf dem Revit-Projekt-Basispunkt zu erstellen und es dann so zu drehen, dass es der Drehung des Grundstücks entspricht. Anschließend können Sie dieses transformierte Koordinatensystem in den Block FamilyInstance.ByCoordinateSystem übertragen, um Familienexemplare unter Berücksichtigung der Ausrichtung des Grundstücks an den beabsichtigten realen Positionen zu platzieren.

Weitere Informationen zu Koordinatensystemen finden Sie weiter unten.
https://help.autodesk.com/view/RVT/2025/DEU/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Beispieldatei

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)

## In profondità
This node creates an elevation view from an existing ElevationMarker by specifying the marker index. Each ElevationMarker in Revit can host up to four individual elevation views—one for each direction (North, South, East, and West). This node lets you generate one of those directional elevations by referencing the marker and the desired index number.

In this example, a Elevation Marker is created and used as input elevationMarker to node ElevationMarker.CreateElevationByMarkerIndex along with a view and index (0,1,2,3).

___
## File di esempio

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)

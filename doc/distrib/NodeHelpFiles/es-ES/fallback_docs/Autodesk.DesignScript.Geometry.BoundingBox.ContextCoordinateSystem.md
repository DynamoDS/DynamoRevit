## En detalle:
`BoundingBox.ContextCoordinateSystem` returns the coordinate system that defines the orientation of the bounding box, allowing Dynamo to represent rotated or locally oriented bounding boxes.

In the example below, a door is selected and its geometry is used to generate a bounding box. This is then used as the input to `BoundingBox.ContextCoordinateSystem`, where the coordinate system is exposed.

___
## Archivo de ejemplo

![BoundingBox.ContextCoordinateSystem](./Autodesk.DesignScript.Geometry.BoundingBox.ContextCoordinateSystem_img.jpg)

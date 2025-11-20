## 詳細
The ReferencePlane.ByLine node in Dynamo creates a Revit reference plane by using a defined line as its base. This allows you to generate custom reference planes at specific positions and orientations.

In this example, two points are defined using Point.ByCoordinates with adjustable sliders. A Line.ByStartPointEndPoint is then created between these two points, and finally, the ReferencePlane.ByLine node generates a reference plane along that line.
___
## サンプル ファイル

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

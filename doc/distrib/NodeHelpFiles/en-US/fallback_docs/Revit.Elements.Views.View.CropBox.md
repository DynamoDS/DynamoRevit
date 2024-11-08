## In Depth
`View.CropBox` obtains the crop box of the given view as a Dynamo bounding box geometry element. Bounding boxes are axis-aligned, which results in the returned geometry having an orientation of X and Y axis. If the crop box is not active for the given view, a bounding box surrounding all geometry of the given view is returned.

In the example below, the crop box of the active view is returned, along with a cuboid representation of the bounding box.
___
## Example File

![View.CropBox](./Revit.Elements.Views.View.CropBox_img.jpg)
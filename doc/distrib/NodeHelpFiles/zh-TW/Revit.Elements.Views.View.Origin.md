## In Depth
Every view in Revit has an origin. `View.Origin` returns this value as a Dynamo point. According to the Revit API documentation, "The origin of a plan view is not meaningful". With this in mind, the value offered by `View.Origin` is up to the end-user.

In the example below, the origin of the active view and a selected 3d view is returned.
___
## Example File

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)
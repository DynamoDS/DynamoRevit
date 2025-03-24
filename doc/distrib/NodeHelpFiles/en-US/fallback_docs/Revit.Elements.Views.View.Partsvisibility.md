## In Depth
`View.Partsvisibility` returns the visibilty setting for parts elements in the given view. Parts in Revit are a way to break up system families into subcomponents. As a result, there are different ways to display the objects. 

Values from this node include:
- Unset - Parts visibility is not set for the view.
- ShowPartsOnly - Show only parts for the view. 
- ShowOriginalOnly - Show original elements, not parts, for the view. 
- ShowPartsAndOriginal -  Show both original elements and parts for the view. 

In the example below, the parts visibility option is returned for the active view.
___
## Example File

![View.Partsvisibility](./Revit.Elements.Views.View.Partsvisibility_img.jpg)
## In Depth
Places Revit Groups with location (points) and groupType as inputs.  Object.Type output for groupType reads Revit.Elements.ElementType

 In the example below, all Model Groups are collected from the active Revit document. The Groups' Types are returned with Group.GroupType, reduced to the first Group in the project list and used as an input for GroupType.  The location input is placed at 100,100.  
___
## Example File

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)
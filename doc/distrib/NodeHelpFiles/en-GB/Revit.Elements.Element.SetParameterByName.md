## In Depth
`Element.SetParameterByName` sets a parameter element (found by name) to a given value. Values include: double, integer, boolean, ElementId, Element and string. In Revit, parameters can share the same name. As a result, `Element.SetParameterByName` will set the value on the first parameter that is found, sorted alphabetically by UniqueId.

In the example below, the comments parameter is being set for all furniture items in the model that are located within a room. The value of the comments parameter is the room name that is obtained.
___
## Example File

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
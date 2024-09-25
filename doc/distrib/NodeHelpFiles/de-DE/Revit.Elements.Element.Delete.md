## In Depth
`Element.Delete` operates the same way as the delete option in the Revit interface. It will delete the input element and any elements that depend on it. 

Eg. deleting a wall with doors in it, will delete the doors as well. The output consists of a nested list of the element ids of the elements that were deleted as a result. Note: this node is best used on manual execution mode in Dynamo.

In the example below, all of the "Help Button" family instances are deleted from the current document (file).
___
## Example File

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)
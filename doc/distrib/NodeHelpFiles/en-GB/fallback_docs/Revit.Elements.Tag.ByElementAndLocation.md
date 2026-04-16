## In Depth
This node tags Revit elements given a view, element, location, horizontal (if no, the tag will orientate based off the element) and addLeader as inputs.

In this example a door is selected in the “Studio Live Work Core B” view.  The location of that door is extracted then used as the original input to Tag.ByElementAndLocation along with Boolean values for horizontal and addLeader.  The original location is modified so that the tag location does not overlay directly on top of the element using the Tag.SetHeadLocation node.

___
## Example File

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

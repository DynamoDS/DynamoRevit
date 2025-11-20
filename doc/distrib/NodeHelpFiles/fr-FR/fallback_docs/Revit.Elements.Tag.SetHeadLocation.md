## Description approfondie
This node takes a tag and changes its head location.  This gives us the ability to automate a consistent placement behavior so that tags are directly on top of the element they are tagging.

In this example a door is selected in the “Studio Live Work Core B” view.  The location of that door is extracted then used as the original input to Tag.ByElementAndLocation along with Boolean values for horizontal and addLeader.  The original location is modified so that the tag location does not overlay directly on top of the element using the Tag.SetHeadLocation node.

___
## Exemple de fichier

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

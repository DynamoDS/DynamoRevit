## In profondità
This node tags Revit elements given a view, element, offset, horizontalAligment, verticalAlignment, horizontal (if no, the tag will orientate based off the element) and addLeader as inputs.

In this example a door is selected in the “Studio Live Work Core B” view and are used as inputs to Tag.ByelementAndOffset.  The location of that door is extracted and used as the vector start point.  The same point is modified using a slider changing the x and y points and used as the vector end point.  This vector is used as our input for offset along with true values in the horizontal and addLeader inputs.  The horizontalAlignment is defined by the Selection Horizontal Text Alignment node drop down values (Left, Center, Right) and the Selection Vertical Text Alignment node drop down values (Bottom, Middle, Top).

___
## File di esempio

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)

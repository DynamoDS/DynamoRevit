## In Depth
This node returns a list of connection nodes filtered by the selected section profile.

When selecting a series of structural data elements with multiple connection nodes, this example filters the list of connection nodes to the selected section type. Structure data elements can have sub lists of items, so the index is set to 0 - the default value.

In the example, a group of W-Shapes are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The value of "accepted" is the connection node filtered by the selected section type.  In the case, the section type is 'W24x104'.

___
## Example File

![Filter.BySectionType](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySectionType_img.jpg)

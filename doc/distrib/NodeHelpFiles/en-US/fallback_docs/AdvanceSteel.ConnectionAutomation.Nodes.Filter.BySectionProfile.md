## In Depth
This node returns a list of connection nodes filtered by the selected section profile.

When selecting a series of structural data elements with multiple connection nodes, filters the list of connection nodes to the selected section profile. Structure data elements can have sub lists of items, so the index is set to 0 - the default value.

In the example, a group of W-Shapes are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The value of "accepted" is the connection node filtered by the selected section profile.  In the case, the section profile is 'W Shapes-Column'.
___
## Example File

![Filter.BySectionProfile](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySectionProfile_img.jpg)
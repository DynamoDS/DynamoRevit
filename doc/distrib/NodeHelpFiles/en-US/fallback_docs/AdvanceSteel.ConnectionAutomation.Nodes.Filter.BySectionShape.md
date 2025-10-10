## In Depth
This node returns a list of connection nodes filtered by the selected section shape.

When selecting a series of structural data elements with multiple connection nodes, this example filters the list of connection nodes to those that match the selected section shape. Structure data elements can have sub lists of items, so the index is set to 0 - the default value.

In the example, a group of I-shaped parallel flanges and a HSS are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is the connection node filtered by the selected section shape.  In the case, the section shape is a `HSS`.
___
## Example File

![Filter.BySectionShape](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySectionShape_img.jpg)
## In Depth

This node returns a list of connection nodes filtered by the selected zone type: end or body.

When selecting a series of structural data elements with multiple connection nodes, this example filters the list of connection nodes with the selected section profile. Structure data elements can have sub lists of items, so the index is set to 0 - the default value.

In the example, a group of I-shaped parallel flanges are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is the connection node filtered by the selected zoneType.  In the case, the zoneType is 'end'.

___
## Example File

![Filter.ByZoneType](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByZoneType_img.jpg)
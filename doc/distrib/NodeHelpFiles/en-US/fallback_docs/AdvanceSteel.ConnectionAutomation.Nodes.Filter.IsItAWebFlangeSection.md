## In Depth
This node returns a list of connection nodes filtered by the selected section shape.

When selecting a connection group that consists of a W shape and HSS, this example filters the list of connection nodes to those that have a web flange section. Structure data elements can have sub lists of items, so the index is set to 0 - the default value.

In the example, a W Shape and a HSS are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is the connection node filtered by the selected section shape.  In the case, the section shape is a `W24X104`.
___
## Example File

![Filter.IsItAWebFlangeSection](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.IsItAWebFlangeSection_img.jpg)
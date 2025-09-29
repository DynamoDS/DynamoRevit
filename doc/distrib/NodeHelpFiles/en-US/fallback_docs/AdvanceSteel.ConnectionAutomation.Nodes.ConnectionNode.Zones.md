## In Depth


This node returns a list of zones within each connection node.

In this example, selecting a structural beam element returns a list of zones. The input is a list of connection nodes, where a connection node groups structure data elements together ( like beams and columns ) in a singular connection.

Within a connection node, the zone represents a specific portion of the structure data element involved in the connection.  The primary zone types are 'end' and 'body'.

End zone represents the end of a structure data element: this is where the connection occurs.

Body zone refers to a location on a structure data element away from the end such as where a brace or member framing into the web of a beam might connect.


___
## Example File

![ConnectionNode.Zones](./AdvanceSteel.ConnectionAutomation.Nodes.ConnectionNode.Zones_img.jpg)
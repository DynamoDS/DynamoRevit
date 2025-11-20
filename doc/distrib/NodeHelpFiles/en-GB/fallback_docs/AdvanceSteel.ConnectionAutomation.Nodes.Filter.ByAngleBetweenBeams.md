## In Depth
This node returns a list of connection nodes filtered by a min-max degree range.

In the example, a W Shape column and an angled HSS brace are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is a connection node the is within the 0-to-60 degree range ( this node measures from X-axis ). If the max value were 45-degrees, the connection node would be rejected.  The `indexFirst` and `indexSecond` params are set to 'use levels' to aligned with the input data structure.
___
## Example File

![Filter.ByAngleBetweenBeams](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenBeams_img.jpg)

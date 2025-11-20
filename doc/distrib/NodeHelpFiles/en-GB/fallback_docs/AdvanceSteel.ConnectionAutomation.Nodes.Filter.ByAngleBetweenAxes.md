## In Depth
This node returns a list of connection nodes filtered by a min-max degree range and the axis selected.

In the example, a W Shape column and an angled HSS brace are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is a connection node the is within the 0-to-60 degree range when comparing the x-axis of both elements. If the max value were 45-degrees, the connection node would be rejected.  The `indexFirst` and `indexSecond` params are set to 'use levels' to aligned with the input data structure.
___
## Example File

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

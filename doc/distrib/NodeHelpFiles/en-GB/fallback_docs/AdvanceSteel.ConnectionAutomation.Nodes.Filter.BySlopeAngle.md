## In Depth
This node returns a list of connection nodes filtered by a min-max degree range of the slant angle.  The slant angle is the angle between the x-axis of a structure data element and the horizontal axis.

In the example, a W Shape column and an angled HSS brace are selected, and the output consists of a list of dictionaries with "accepted" and "rejected" properties.  The accepted value is a connection node that has a slant angle within the 30-to-60 degree . The angled HSS brace has a ~60-degree slope angle, so its accepted. The W Shape column has a slope angle of 90-degree and is rejected.
___
## Example File

![Filter.BySlopeAngle](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySlopeAngle_img.jpg)

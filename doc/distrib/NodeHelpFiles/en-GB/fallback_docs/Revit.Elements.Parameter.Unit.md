## In Depth

Returns a Parameters’ Unit Type.

In the example below, we're extracting all element parameters and using them as input. The goal is to display the unit type for each parameter.
For instance, if Element.Parameters shows "Base Diameter : 1’ – 3 ¼”", the output from Parameter.Unit will be "Unit (Name = Feet or Meters)".
If a parameter doesn't have a unit (like Element.Parameters = Apply Cut : No), then Parameter.Unit will return null.
Since Dynamo itself is unitless, it's crucial to identify incoming unit types to perform accurate calculations. This node helps Dynamo recognize and process that unit data.

___
## Example File

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

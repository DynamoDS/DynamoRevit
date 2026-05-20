## In Depth
The Gate node controls whether input data is allowed to pass through based on a Boolean value. If the gate is open (true), the input is passed to the output. If the gate is closed (false), the input is blocked.

In this example, the user does not need to explode geometry every time the graph runs. When the gate is open (true), newly created geometry is allowed to pass through and is exploded. When the gate is closed (false), the geometry is blocked, so it is not passed on or exploded again.
___
## Example File

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)
## Im Detail
`Gate` controls whether input data is allowed to pass through based on a Boolean value. If the gate is open (True), the input is passed to the output. If the gate is closed (False), the input is blocked.

In the example below, `Gate` is used to prevent geometry from being exploded on every graph run. When the gate is open (True), newly created geometry passes through and is exploded. When the gate is closed (False), the geometry is blocked and not passed on or exploded again.
___
## Beispieldatei

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)

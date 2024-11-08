## In Depth
`Dimension.ValueOverride` returns the value override of the given dimension, if it has an overriden value. For multi-segment dimensions a nested list of values is returned. If the dimension does not have an overriden value, a null value is returned.

In the example below, the first dimension is collected from the active view and checked if it has an overriden dimension. If the dimension is overriden, it is cleared with the `Dimension.SetValueOverride`.
___
## Example File

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)
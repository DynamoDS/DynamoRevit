## In Depth
This node stores a value in memory so it can be recalled later within the same Dynamo session or graph execution context. It is useful when you need to preserve data between runs, maintain state, or reference previously stored information without recreating it each time.

In this example a single floor is selected and the geometry is extracted and then used as the input in Remember node.  The results of the Remember node will be stored here in this output and can be reused.
___
## Example File

![Remember](./CoreNodeModels.Remember_img.jpg)
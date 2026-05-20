## In Depth
This node stores a value in the Dynamo file so it can be recalled later by the graph, including when the input is null. It is useful when you need to preserve data between runs, maintain state, or reference previously stored information without recreating it each time.

In this example a single floor is selected and the geometry is extracted and then used as the input in Remember node.  The results of the Remember node will be stored here in this output and can be reused.
___
## Example File

![Remember](./CoreNodeModels.Remember_img.jpg)
## Informacje szczegółowe
`Remember` stores a value in the Dynamo file so it can be recalled later by the graph, including when the input is null. It is useful for preserving data between runs, maintaining state, or referencing previously stored information without recreating it each time.

In the example below, a single floor is selected and the geometry is extracted and used as the input for `Remember`. The stored result is then available as output for reuse in the graph.
___
## Plik przykładowy

![Remember](./CoreNodeModels.Remember_img.jpg)

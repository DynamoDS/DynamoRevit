## In Depth
`PathOfTravel.LongestOfShortestExitPaths` creates a path of travel representing the longest shortest route from a group of points to one or more exit points. Each start point is tested against the available exits, and Revit determines the closest valid exit path for each one. The node then returns the path with the greatest travel distance among those shortest-path results.

In the example below, a level and floor plan view are first created to provide the required Revit view for the path of travel calculation. Several points are generated using `Point.ByCoordinates` and combined into a list representing possible exit locations. `PathOfTravel.LongestOfShortestExitPaths` uses the floor plan view and the list of destination points to calculate travel paths, returning the path with the greatest travel distance.
___
## Example File

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)
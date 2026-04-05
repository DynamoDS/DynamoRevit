## In Depth
This node retrieves the Revit Performance Adviser rules available in the current session. These rules represent model health checks that Revit can use to identify elements or conditions that may cause warnings or performance issues.

In this example, "Overlapping walls" and "Room separation line is not joined" are selected from the drop menu of node Performance Adviser Rules and added to a list.  This list is used as an input to extract the exact performance of that selected rule within the current file.  Other menu options are, Duplicate instances, Host contains too many inserts, In-place family contains disjoined solids, Interior categories are enabled in 3D view, Many Unused Nested Families, Multiple non-overlapping loops, Project contains unused families and types, Sketch area too large, Sketch is too complex, Too Large Family FIle. 
___
## Example File

![Performance Adviser Rules](./DSRevitNodesUI.PerformanceAdviserRules_img.jpg)
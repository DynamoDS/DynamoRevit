## In Depth
This node retrieves the Revit Performance Adviser rules available in the current session. These rules represent model health checks that Revit can use to identify elements or conditions that may cause warnings or performance issues.

In this example, "Overlapping walls" and "Room separation line is not joined" are selected from the drop-down menu of the Performance Adviser Rules node and added to a list. This list is then used as input to retrieve the selected performance rules for the current file. Other menu options include rules such as "Duplicate instances," "Host contains too many inserts," "Sketch is too complex," and "Too Large Family File."
___
## Example File

![Performance Adviser Rules](./DSRevitNodesUI.PerformanceAdviserRules_img.jpg)
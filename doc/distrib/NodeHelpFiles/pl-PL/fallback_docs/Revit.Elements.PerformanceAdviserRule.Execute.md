## Informacje szczegółowe
This node runs a specific Performance Adviser rule against a set of Revit elements.

In this example the Performance Adviser Rule checks if “View clipping is disabled”. The results are passed into the FailureMessage.FailingElements node, which outputs the specific elements in the model that failed this check. This workflow makes it easier for users to trace and fix the exact elements responsible for issues.

___
## Plik przykładowy

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)

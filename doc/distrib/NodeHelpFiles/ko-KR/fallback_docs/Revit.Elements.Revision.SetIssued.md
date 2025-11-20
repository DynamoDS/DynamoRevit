## 상세
The Revision.SetIssued node in Dynamo allows you to control whether a selected revision in Revit is marked as issued or not issued. It takes a revision element and a boolean input (True/False), giving users direct control over the revision status without manually editing it in Revit.
In this graph, the Select Revision node is used to pick a specific revision (e.g., "Seq. 1 – Schematic Design"). The Boolean node provides a True/False value, which is then connected to the Revision.SetIssued node to update the revision’s issued status automatically.

___
## 예제 파일

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)

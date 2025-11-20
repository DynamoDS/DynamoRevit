## 详细
The Revision.Issued node in Dynamo is used to check whether a revision in Revit is marked as issued. It returns a true or false value (Boolean), helping teams quickly verify the status of revisions without opening the Revit revision settings.

In this graph, the Select Revision node is used to choose a revision from the project. The Revision.Issued node then checks if the selected revision is issued, and the result is displayed in the Watch node as either true or false. This makes it easy to confirm the issue status of a revision directly through Dynamo.

___
## 示例文件

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)

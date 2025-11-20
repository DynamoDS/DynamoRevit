## Description approfondie
The Revision.SetIssuedBy node in Dynamo is used to set or update the “Issued By” value for a revision in Revit. It helps automate revision control by recording who issued the revision, ensuring documentation is clear and consistent without manual edits in Revit.

In this graph, the Select Revision node is used to pick the required revision, and a string input (e.g., ABC) provides the issuer’s name. The Revision.SetIssuedBy node then applies this value to the selected revision, updating the “Issued By” field directly in the Revit model.

___
## Exemple de fichier

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)

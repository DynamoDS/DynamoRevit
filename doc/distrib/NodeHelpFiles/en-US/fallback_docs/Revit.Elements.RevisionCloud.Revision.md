## In Depth
This node extracts the revision element linked to a specific revision cloud in Revit. It provides the revision data associated with that cloud, allowing users to check, track, or validate revision details programmatically within their project.

In this example a rectangle is created with number sliders for width and length, then exploded into curves and reversed for proper orientation. These curves, along with a chosen view (L1_SD) and a selected revision (Seq. 2 – Not For Construction), are used to generate a revision cloud via the RevisionCloud.ByCurve node. The resulting revision cloud is connected to the RevisionCloud.Revision node, which retrieves and outputs the revision associated with that cloud. This ensures that users can confirm which revision is tied to each revision cloud.
___
## Example File

![RevisionCloud.Revision](./Revit.Elements.RevisionCloud.Revision_img.jpg)
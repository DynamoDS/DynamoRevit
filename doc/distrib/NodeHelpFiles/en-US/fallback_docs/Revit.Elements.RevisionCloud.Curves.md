## In Depth
This node extracts the start points and end points (line) of the input revisionCloud.

In this example, a rectangle is created using number sliders to define its dimensions, which is then exploded into curves and reversed for orientation. These curves, along with a selected view (Site Plan) and revision (Seq. 2 – Not For Construction), are used to generate a revision cloud with the RevisionCloud.ByCurve node. The created revision cloud is then connected to the RevisionCloud.Curves node, which extracts and displays the defining curves of that cloud. This helps users verify the geometry of the revision cloud and provides flexibility for reuse or further automation.
___
## Example File

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
## Im Detail
This node extracts the curve loops (typically arcs and lines) that make up the visible perimeter of a Revision Cloud. Each segment of the cloud is represented as a curve object (usually an Arc) corresponding to the “bubbled” shape of the revision mark in a view or sheet.

In this example, a rectangle is created using number sliders to define its dimensions, which is then exploded into curves and reversed for orientation. These curves, along with a selected view (Site Plan) and revision (Seq. 2 – Not For Construction), are used to generate a revision cloud with the RevisionCloud.ByCurve node. The created revision cloud is then connected to the RevisionCloud.Curves node, which extracts and displays the defining curves of that cloud. This helps users verify the geometry of the revision cloud and provides flexibility for reuse or further automation.
___
## Beispieldatei

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)

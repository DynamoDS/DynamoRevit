## In profondità
This node creates a revision cloud in a specified view.  The inputs are view , list of curves "cloud outline," and a defined revision.

In this example, two number sliders define the rectangle’s width and length, which is then exploded into curves. These curves are reversed to maintain correct orientation and then connected to the RevisionCloud.ByCurve node. The graph also takes the active view (First Floor Plan) and the chosen revision (Seq. 2 – Not For Construction) as inputs. Together, it automatically generates a revision cloud in the selected view based on the defined shape.
___
## File di esempio

![RevisionCloud.ByCurve](./Revit.Elements.RevisionCloud.ByCurve_img.jpg)

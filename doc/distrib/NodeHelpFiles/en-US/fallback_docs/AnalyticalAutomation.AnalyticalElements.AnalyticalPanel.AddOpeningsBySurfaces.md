## In Depth
This node adds openings to an analytical panel using defined surfaces as the shape of each opening. Existing inputs for updating geometry, parameters, and associations can be applied as needed.

In this example, four points at grid intersections A4–A5 and B4–B5 in the Snowdon Structural model define the opening’s location and level (L2 TOS) sets its vertical placement. Lines connect the points and create an offset surface representing the opening. A floor at L2 TOS is used to create the analytical panel, and this node links the opening surface to the panel, generating the corresponding analytical opening.
___
## Example File

![AnalyticalPanel.AddOpeningsBySurfaces](./AnalyticalAutomation.AnalyticalElements.AnalyticalPanel.AddOpeningsBySurfaces_img.jpg)
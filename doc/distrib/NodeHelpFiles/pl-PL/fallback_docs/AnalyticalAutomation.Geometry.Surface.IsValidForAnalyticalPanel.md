## Informacje szczegółowe
This node evaluates a given surface to determine if it is valid for use in defining an analytical panel. A valid surface is typically planar, continuous, and suitable for conversion into an analytical representation within Revit’s analytical model environment.

In this example, the faces of a slab element from the project are collected, and the top face is provided to the node as input. The node returns a Boolean result indicating whether the selected surface meets the requirements for creating an analytical panel, along with an optional message describing any issues encountered during validation.
___
## Plik przykładowy

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

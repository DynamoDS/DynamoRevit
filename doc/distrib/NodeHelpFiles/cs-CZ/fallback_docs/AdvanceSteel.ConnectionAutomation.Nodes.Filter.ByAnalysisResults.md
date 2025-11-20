## Podrobnosti
This node filters a list of ConnectionNodes by checking whether the force value at a specified index falls within a defined range. The force data comes from either structural analysis results or the Revit analytical model, and is filtered by the selected result type (e.g., Fx, Fy, Fz, Mx, My, Mz).

In this example, a set of column elements is selected and evaluated based on the Fz force component, using the chosen analysis result and load case. Only those elements whose Fz value falls within the specified force range are returned as accepted connections.
___
## Vzorový soubor

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

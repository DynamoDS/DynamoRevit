## En detalle:
`VectorAnalysisDisplay.ByViewPointsAndVectorValues` creates a vector analysis display in a Revit view. The input view defines where the analysis display will appear. The points define the locations of the analysis markers, the vectors define the direction of the displayed analysis, and the values define the numeric result associated with each point. The output is a Revit analysis display element shown in the selected view.

In the example below, 2 points are created to define sample locations in a Revit 3D view. 2 vectors are then created to define directional analysis data at those locations. The points and vectors are combined into lists and used as inputs to `VectorAnalysisDisplay.ByViewPointsAndVectorValues` along with a view, analysis name, and description. The output is a vector analysis display shown in the selected Revit view.
___
## Archivo de ejemplo

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)

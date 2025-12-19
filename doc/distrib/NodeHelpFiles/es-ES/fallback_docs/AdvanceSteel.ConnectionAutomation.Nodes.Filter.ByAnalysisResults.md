## En detalle:
Este nodo filtra una lista de ConnectionNodes comprobando si el valor de fuerza en un índice especificado se encuentra dentro de un rango definido. Los datos de la fuerza proceden de los resultados del análisis estructural o del modelo analítico de Revit, y se filtran por el tipo de resultado seleccionado (p. ej., Fx, Fy, Fz, Mx, My y Mz).

En este ejemplo, se selecciona un conjunto de elementos de pilar y se evalúa en función del componente de fuerza Fz mediante el resultado del análisis y el caso de carga elegidos. Solo aquellos elementos cuyo valor Fz se encuentre dentro del rango de fuerza especificado se devuelven como conexiones aceptadas.
___
## Archivo de ejemplo

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)

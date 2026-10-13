## En detalle:
`VectorAnalysisDisplay.ByViewPointsAndVectorValues` crea una visualización de análisis vectorial en una vista de Revit. La vista de entrada define dónde aparecerá la visualización del análisis. Los puntos definen las ubicaciones de los marcadores de análisis, los vectores definen la dirección del análisis mostrado y los valores definen el resultado numérico asociado a cada punto. El resultado es un elemento de visualización de análisis de Revit que se muestra en la vista seleccionada.

En el ejemplo siguiente, se crean dos puntos para definir las ubicaciones de muestra en una vista 3D de Revit. A continuación, se crean dos vectores para definir los datos del análisis direccional en esas ubicaciones. Los puntos y los vectores se agrupan en listas y se utilizan como datos de entrada para `VectorAnalysisDisplay.ByViewPointsAndVectorValues`, junto con una vista, el nombre del análisis y una descripción. El resultado es una visualización del análisis vectorial que se muestra en la vista seleccionada de Revit.
___
## Archivo de ejemplo

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)

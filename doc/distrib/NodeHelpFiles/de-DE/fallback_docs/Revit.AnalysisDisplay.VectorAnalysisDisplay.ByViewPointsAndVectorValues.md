## Im Detail
`VectorAnalysisDisplay.ByViewPointsAndVectorValues` erstellt eine Vektoranalyseanzeige in einer Revit-Ansicht. Die Eingabeansicht definiert, wo die Analyseanzeige positioniert wird. Die Punkte definieren die Positionen der Analysemarkierungen, die Vektoren definieren die Richtung der angezeigten Analyse, und die Werte definieren das numerische Ergebnis, das den einzelnen Punkten zugeordnet ist. Die Ausgabe ist ein Anzeigeelement für die Revit-Analyse, das in der ausgewählten Ansicht angezeigt wird.

Im folgenden Beispiel werden zwei Punkte erstellt, um Beispielpositionen in einer Revit-3D-Ansicht zu definieren. Anschließend werden zwei Vektoren erstellt, um die Richtungsanalysedaten an diesen Positionen zu definieren. Die Punkte und Vektoren werden in Listen zusammengefasst und zusammen mit einer Ansicht, einem Analysenamen und einer Beschreibung als Eingaben für `VectorAnalysisDisplay.ByViewPointsAndVectorValues` verwendet. Die Ausgabe ist eine Vektoranalyse-Ansicht, die in der ausgewählten Revit-Ansicht angezeigt wird.
___
## Beispieldatei

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)

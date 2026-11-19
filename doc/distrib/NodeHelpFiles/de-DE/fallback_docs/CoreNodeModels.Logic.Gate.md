## Im Detail
`Gate` steuert, ob Eingabedaten basierend auf einem booleschen Wert übergeben werden dürfen. Wenn das Gatter offen ist (True), wird die Eingabe an die Ausgabe übergeben. Wenn das Gatter geschlossen ist (False), wird die Eingabe blockiert.

Im folgenden Beispiel wird `Gate` verwendet, um zu verhindern, dass Geometrie bei jeder Diagrammausführung aufgelöst wird. Wenn das Gatter offen ist (True), wird neu erstellte Geometrie übergeben und aufgelöst. Wenn das Gatter geschlossen ist (False), wird die Geometrie blockiert und nicht erneut übergeben oder aufgelöst.
___
## Beispieldatei

![Gate](./CoreNodeModels.Logic.Gate_img.jpg)

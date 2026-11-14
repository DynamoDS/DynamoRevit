## Im Detail
`ElementFaceReference.BySurface` erstellt eine Revit-Flächenreferenz aus einer ausgewählten oder generierten Fläche. Eine mit Revit-Geometrie verknüpfte Dynamo-Fläche wird in ein `ElementFaceReference`-Objekt konvertiert. Diese Referenz kann dann von anderen Blöcken verwendet werden, die eine Revit-Flächenreferenz statt nur Oberflächengeometrie benötigen.

Im folgenden Beispiel wird eine Wand erstellt, und eine Fläche aus dieser Wand wird als Eingabe für `ElementFaceReference.BySurface` verwendet. Die Ausgabe ist ein `ElementFaceReference`-Objekt, das von Blöcken verwendet werden kann, die eine Referenz auf eine Revit-Elementfläche benötigen.
___
## Beispieldatei

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)

## Im Detail
`SpecType.ByTypeId` konvertiert eine Zeichenfolge für die Revit-Rohrklassen-Typ-ID in ein `SpecType`-Objekt. Rohrklassentypen definieren die Art von Daten, die ein Parameter in Revit darstellt, z. B. Länge, Fläche, Winkel, Material, Kraft, Text, Ganzzahl und vieles mehr. Dies wird häufig verwendet, wenn gemeinsam genutzte Parameter, Projektparameter oder Parameterdefinitionen in neueren Versionen von Revit erstellt werden, in denen Autodesk das ältere `ParameterType`-System durch Forge-Typ-IDs ersetzt hat.

Im folgenden Beispiel wird eine Zeichenfolge, die eine Revit-Rohrklassen-Typ-ID darstellt, für `SpecType.ByTypeId` bereitgestellt. Die Ausgabe ist ein `SpecType`-Objekt, das später beim Erstellen einer neuen Parameterdefinition verwendet werden kann, damit Revit erkennt, welche Art von Daten der Parameter speichern soll.
___
## Beispieldatei

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

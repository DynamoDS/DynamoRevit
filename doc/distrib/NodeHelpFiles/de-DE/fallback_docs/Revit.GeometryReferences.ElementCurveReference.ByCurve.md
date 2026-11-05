## Im Detail
`ElementCurveReference.ByCurve` erstellt eine Revit-Elementkurvenreferenz aus einer Dynamo-Kurve. Eine Kurvenreferenz ist nützlich, wenn ein anderer Revit-Block statt nur Dynamo-Geometrie eine auswählbare oder referenzierbare Kurve benötigt. Dies wird häufig für Arbeitsabläufe verwendet, die Revit-Referenzen erfordern, z. B. zum Erstellen von Bemaßungen, Ausrichtungen, Abhängigkeiten oder anderen Elementen, die Modellgeometrie referenzieren müssen.

Im folgenden Beispiel wird eine Kurve ausgewählt und als Eingabe für `ElementCurveReference.ByCurve` verwendet. Die Ausgabe ist ein `ElementCurveReference`-Objekt, das dann als Revit-Kurvenreferenz für nachgelagerte Blöcke verwendet wird, die kurvenbasierte Referenzen erfordern.
___
## Beispieldatei

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

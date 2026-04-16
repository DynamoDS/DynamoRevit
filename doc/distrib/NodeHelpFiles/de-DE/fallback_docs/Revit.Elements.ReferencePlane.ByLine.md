## Im Detail
Der Block ReferencePlane.ByLine in Dynamo erstellt eine Revit-Referenzebene, indem eine definierte Linie als Basis verwendet wird. Auf diese Weise können Sie benutzerdefinierte Referenzebenen an bestimmten Positionen und mit bestimmten Ausrichtungen erstellen.

In diesem Beispiel werden zwei Punkte mithilfe von Point.ByCoordinates mit anpassbaren Schiebereglern definiert. Anschließend wird zwischen diesen beiden Punkten ein Line.ByStartPointEndPoint-Objekt erstellt, und schließlich generiert der ReferencePlane.ByLine-Block eine Referenzebene entlang dieser Linie.
___
## Beispieldatei

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

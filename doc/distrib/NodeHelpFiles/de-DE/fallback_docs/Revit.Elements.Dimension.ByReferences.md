## Im Detail
`Dimension.ByReferences` platziert eine Bemaßung entlang der Eingabelinie in der ausgewählten Ansicht, wobei die angegebenen Referenzen verwendet werden. Die Ausgabe ist ein Revit-Bemaßungselement.

Im folgenden Beispiel werden mehrere Punkte erstellt und mit Linienblöcken verbunden, um Modellkurven in Revit zu generieren. Die Modellkurven werden dann in Revit-Kurvenreferenzen konvertiert. Die Referenzen werden in einer Liste zusammengefasst und als `references`-Eingabe für `Dimension.ByReferences` verwendet. Zusätzliche Linien werden erstellt, um die Platzierung und Richtung der Bemaßungen zu definieren. Die aktive Revit-Ansicht wird außerdem als `view`-Eingabe bereitgestellt.
___
## Beispieldatei

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

## Im Detail
`Element.Solids` gibt die Volumengeometrie für das angegebene Element zurück. Die Volumenkörper werden als verschachtelte Listen zurückgegeben, da ein angegebenes Element mehrere Volumenkörper enthalten kann. Wenn ein einzelner Volumenkörper, der das Element darstellt, gewünscht wird, kann `Solid.ByUnion` für die Listen verwendet werden.

Im folgenden Beispiel werden alle Wände aus der ausgewählten Ansicht gesammelt. Wände, die als Projektfamilien erstellt wurden, werden dann entfernt, und die Volumenkörper der übrigen Wände werden zurückgegeben.

___
## Beispieldatei

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

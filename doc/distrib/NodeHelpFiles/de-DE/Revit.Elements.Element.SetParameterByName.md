## Im Detail
`Element.SetParameterByName` setzt ein Parameterelement (gefunden anhand des Namens) auf einen bestimmten Wert. Die Werte umfassen: double, integer, boolean, ElementId, Element und string. In Revit können Parameter denselben Namen haben. Daher wird der Wert von `Element.SetParameterByName` für den ersten gefundenen Parameter festgelegt und alphabetisch nach UniqueId sortiert.

Im folgenden Beispiel wird der Kommentarparameter für alle Möbelelemente im Modell festgelegt, die sich in einem Raum befinden. Der Wert des Kommentarparameters ist der abgerufene Raumname.
___
## Beispieldatei

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

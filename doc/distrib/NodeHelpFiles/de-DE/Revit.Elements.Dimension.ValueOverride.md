## Im Detail
`Dimension.ValueOverride` gibt die Wertüberschreibung der angegebenen Bemaßung zurück, wenn diese einen überschriebenen Wert aufweist. Bei Multi-Segmentbemaßungen wird eine verschachtelte Liste mit Werten zurückgegeben. Wenn die Bemaßung keinen überschriebenen Wert aufweist, wird ein Nullwert zurückgegeben.

Im folgenden Beispiel wird die erste Bemaßung aus der aktiven Ansicht gesammelt und überprüft, ob sie eine überschriebene Bemaßung aufweist. Wenn die Bemaßung überschrieben wurde, wird sie mit `Dimension.SetValueOverride` gelöscht.
___
## Beispieldatei

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)

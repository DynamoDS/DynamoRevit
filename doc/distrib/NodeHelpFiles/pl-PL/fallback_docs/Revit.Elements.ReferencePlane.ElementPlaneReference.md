## Informacje szczegółowe
Ten węzeł wyodrębnia rzeczywiste odniesienie elementu programu Revit dla wybranej płaszczyzny odniesienia. Jest to przydatne, gdy trzeba użyć tej płaszczyzny jako odniesienia nadrzędnego dla geometrii lub wymiarów w programie Revit.

Przykład:
Na tym wykresie definiowane są dwa punkty przy użyciu współrzędnych i tworzona jest płaszczyzna odniesienia między nimi za pomocą węzła ReferencePlane.ByStartPointEndPoint. Ta płaszczyzna odniesienia jest następnie łączona z węzłem ReferencePlane.ElementPlaneReference, który generuje odniesienie natywne dla programu Revit, dzięki czemu jest ona gotowa do pełnienia roli obiektu nadrzędnego lub użycia w zadaniach wyrównywania.
___
## Plik przykładowy

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

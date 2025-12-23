## Informacje szczegółowe
Węzeł ReferencePlane.ByLine w dodatku Dynamo tworzy płaszczyznę odniesienia programu Revit przy użyciu zdefiniowanej linii jako podstawy. Umożliwia to generowanie niestandardowych płaszczyzn odniesienia w określonych położeniach i orientacjach.

W tym przykładzie definiowane są dwa punkty za pomocą węzła Point.ByCoordinates i regulowanych suwaków. Między tymi dwoma punktami jest następnie tworzona linia Line.ByStartPointEndPoint, a na koniec węzeł ReferencePlane.ByLine generuje wzdłuż tej linii płaszczyznę odniesienia.
___
## Plik przykładowy

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)

## Informacje szczegółowe
Ten węzeł pobiera odniesienie krzywej skojarzone z danym elementem CurveElement programu Revit, takim jak krzywa modelu lub linia szczegółu. Odniesienie może być następnie używane jako dane wejściowe dla innych węzłów, które wymagają geometrii odniesienia, takich jak służące do wymiarowania, wyrównywania lub tworzenia podzielonych ścieżek.

W tym przykładzie zostaje utworzona krzywa modelu przy użyciu punktu początkowego i punktu końcowego, a następnie zostaje ona przekazana jako dane wejściowe do węzła CurveElement.ElementCurveReference. Dane wyjściowe to odniesienie geometryczne elementu krzywej, które może być używane w kolejnych operacjach.
___
## Plik przykładowy

![CurveElement.ElementCurveReference](./Revit.Elements.CurveElement.ElementCurveReference_img.jpg)

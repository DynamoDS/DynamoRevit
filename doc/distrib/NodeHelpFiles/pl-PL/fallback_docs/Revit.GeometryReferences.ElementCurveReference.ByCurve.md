## Informacje szczegółowe
Węzeł ElementCurveReference.ByCurve tworzy odniesienie krzywej elementu programu Revit na podstawie krzywej dodatku Dynamo. Odniesienie krzywej jest przydatne, gdy inny węzeł programu Revit wymaga krzywej wybieralnej lub z możliwością odniesienia, a nie tylko geometrii dodatku Dynamo. Jest to często używane w przypadku procesów wymagających odniesień programu Revit, takich jak tworzenie wymiarów, linii trasowania, wiązań lub innych elementów, które muszą odnosić się do geometrii modelu.

W poniższym przykładzie krzywa jest wybierana używana jako dane wejściowe węzła ElementCurveReference.ByCurve. Dane wyjściowe to obiekt ElementCurveReference, który jest następnie używany jako odniesienie krzywej programu Revit dla kolejnych węzłów wymagających odniesień opartych na krzywych.
___
## Plik przykładowy

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)

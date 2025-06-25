## Informacje szczegółowe
Węzeł `ReferencePoint.ByPoint` tworzy element punktu odniesienia w aktywnym dokumencie rodziny programu Revit w podanym położeniu punktu. Uwaga: dokument rodziny musi być komponentem adaptacyjnym lub rodziną brył. Ten węzeł różni się od węzła „ReferencePoint.ByCoordinates”, ponieważ jako położenie używany jest punkt dodatku Dynamo. Jest to przydatne, ponieważ użytkownik końcowy może użyć bezpośredniej manipulacji do edycji punktu Dynamo, co spowoduje zaktualizowanie również punktu odniesienia. Więcej informacji na temat bezpośredniej manipulacji można znaleźć na stronie, do której prowadzi ten [link](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

W poniższym przykładzie zostaje utworzony nowy punkt odniesienia o współrzędnych 0,0,1.
___
## Plik przykładowy

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)

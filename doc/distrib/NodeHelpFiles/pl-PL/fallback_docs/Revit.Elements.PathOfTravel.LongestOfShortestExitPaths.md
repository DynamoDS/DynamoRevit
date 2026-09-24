## Informacje szczegółowe
Węzeł PathOfTravel.LongestShortestExitPaths tworzy ścieżkę przejścia reprezentującą najdłuższą najkrótszą trasę z grupy punktów do co najmniej jednego punktu wyjścia. Każdy punkt początkowy jest testowany względem dostępnych wyjść i program Revit określa najbliższą prawidłową ścieżkę wyjścia dla każdego z nich. Następnie węzeł zwraca ścieżkę o największej odległości przebycia spośród wyników dla najkrótszej ścieżki.

W poniższym przykładzie najpierw jest tworzony widok poziomu i rzutu kondygnacji, aby zapewnić wymagany widok programu Revit do obliczenia ścieżki przejścia. Za pomocą węzła Point.ByCoordinates jest generowanych kilka punktów, które są łączone w listę reprezentującą możliwe położenia zjazdów. Węzeł PathOfTravel.LongestOfShortestExitPaths oblicza ścieżki podróży na podstawie rzutu kondygnacji i listy punktów docelowych, zwracając ścieżkę o największej odległości do pokonania.
___
## Plik przykładowy

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)

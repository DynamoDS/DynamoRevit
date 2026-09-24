## Informacje szczegółowe
Węzeł SpecType.ByTypeId konwertuje ciąg identyfikatora Spec TypeId programu Revit na obiekt SpecType. Typy specyfikacji definiują rodzaj danych parametru reprezentowanych w programie Revit, takich jak długość, powierzchnia, kąt, materiał, siła, tekst czy liczba całkowita. Jest to często używane podczas tworzenia parametrów współdzielonych, parametrów projektu lub definicji parametrów w nowszych wersjach programu Revit, w których firma Autodesk zastąpiła starszy system ParameterType identyfikatorami typu Forge.

W poniższym przykładzie do węzła SpecTypeId zostaje przekazany ciąg reprezentujący identyfikator Spec TypeId programu Revit. Dane wyjściowe to obiekt SpecType, który może być używany później podczas tworzenia nowej definicji parametru, aby program Revit rozumiał typ danych, które parametr powinien przechowywać.
___
## Plik przykładowy

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)

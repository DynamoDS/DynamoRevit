## Informacje szczegółowe

Zwraca typ jednostki parametrów.

W poniższym przykładzie wyodrębniamy wszystkie parametry elementów i używamy ich jako danych wejściowych. Celem jest wyświetlenie typu jednostki dla każdego parametru.
Jeśli na przykład Element.Parameters wskazuje wartość „Base Diameter : 1’ – 3 ¼””, pozycja danych wyjściowych z węzła Parameter.Unit to „Unit (Name = Feet or Meters)”.
Jeśli parametr nie ma jednostki (np. Element.Parameters = Apply Cut : No), węzeł Parameter.Unit zwróci wartość null.
Ponieważ sam dodatek Dynamo nie używa jednostek, kluczowe jest zidentyfikowanie przychodzących typów jednostek w celu wykonania dokładnych obliczeń. Ten węzeł pomaga dodatkowi Dynamo rozpoznawać i przetwarzać dane jednostek.

___
## Plik przykładowy

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)

## Informacje szczegółowe
Węzeł `Element.Delete` działa tak samo jak opcja usuwania w interfejsie programu Revit. Powoduje usunięcie elementu wejściowego i wszystkich elementów od niego zależnych.

Na przykład usunięcie ściany z drzwiami spowoduje również usunięcie drzwi. Wynik składa się z zagnieżdżonej listy identyfikatorów elementów, które zostały usunięte w wyniku tego procesu. Uwaga: tego węzła najlepiej używać w trybie wykonywania ręcznego w dodatku Dynamo.

W poniższym przykładzie usuwane są wszystkie wystąpienia rodziny „Przycisk pomocy” z bieżącego dokumentu (pliku).
___
## Plik przykładowy

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

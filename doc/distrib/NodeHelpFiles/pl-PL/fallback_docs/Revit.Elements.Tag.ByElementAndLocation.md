## Informacje szczegółowe
Ten węzeł oznacza elementy programu Revit na podstawie danych wejściowych view, element, location, horizontal (jeśli ta pozycja orientacji w poziomie ma wartość negatywną, oznaczenie zostanie zorientowane na podstawie orientacji elementu) i addLeader.

W tym przykładzie w widoku „Studio Live Work Core B” wybierane są drzwi. Lokalizacja tych drzwi jest wyodrębniana, a następnie używana jako pozycja danych wejściowych original węzła Tag.ByElementAndLocation wraz z wartościami logicznymi (Boolean) parametrów horizontal i addLeader. Pierwotne położenie (original) jest modyfikowane w taki sposób, aby położenie oznaczenia nie nakładało się bezpośrednio na element, za pomocą węzła Tag.SetHeadLocation.

___
## Plik przykładowy

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

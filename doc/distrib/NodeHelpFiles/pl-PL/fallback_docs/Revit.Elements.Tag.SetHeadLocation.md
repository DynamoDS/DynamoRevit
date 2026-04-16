## Informacje szczegółowe
Ten węzeł pobiera oznaczenie i zmienia położenie jego nagłówka. Pozwala to zautomatyzować spójne zachowanie umieszczania, tak aby oznaczenia znajdowały się bezpośrednio na elemencie, który jest oznaczany.

W tym przykładzie w widoku „Studio Live Work Core B” wybierane są drzwi. Lokalizacja tych drzwi jest wyodrębniana, a następnie używana jako pozycja danych wejściowych original węzła Tag.ByElementAndLocation wraz z wartościami logicznymi (Boolean) parametrów horizontal i addLeader. Pierwotne położenie (original) jest modyfikowane w taki sposób, aby położenie oznaczenia nie nakładało się bezpośrednio na element, za pomocą węzła Tag.SetHeadLocation.

___
## Plik przykładowy

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)

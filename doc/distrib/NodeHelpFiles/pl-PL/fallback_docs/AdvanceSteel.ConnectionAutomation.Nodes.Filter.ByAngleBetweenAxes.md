## Informacje szczegółowe
Ten węzeł zwraca listę węzłów połączeń przefiltrowanych według zakresu stopni od wartości minimalnej do maksymalnej oraz wybranej osi.

W przykładzie wybrano kolumnę w kształcie W i kątowe połączenie o profilu zamkniętym, a wynikiem jest lista słowników z właściwościami „accepted” (zaakceptowane) i „rejected” (odrzucone). Wartością akceptowaną jest węzeł połączenia, którego wartość wynosi od 0 do 60 stopni w przypadku porównywania osi X obu elementów. Gdyby maksymalna wartość wynosiła 45 stopni, węzeł połączenia zostałby odrzucony. Parametry „indexFirst” (indeksujPierwsze) i „indexSecond” (indeksujDrugie) są ustawione na „use levels” (użyj poziomów) w celu dopasowania do struktury danych wejściowych.
___
## Plik przykładowy

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)

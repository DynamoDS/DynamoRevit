## Informacje szczegółowe
Ten węzeł zwraca listę węzłów połączeń przefiltrowanych według zakresu kąta pochylenia od wartości minimalnej do maksymalnej. Kąt pochylenia to kąt między osią X elementu danych konstrukcyjnych a osią pionową.

W tym przykładzie wybrano kolumnę w kształcie W i kątowe połączenie o profilu zamkniętym, a wynikiem jest lista słowników z właściwościami „accepted” (zaakceptowane) i „rejected” (odrzucone). Akceptowaną wartością jest węzeł połączenia o nachyleniu kąta w zakresie od 30 do 60 stopni. Kątowe połączenie o profilu zamkniętym ma kąt pochylenia około 30 stopni, więc jest akceptowane. Słup o kształcie W ma kąt pochylenia równy 0 stopni i został odrzucony.
___
## Plik przykładowy

![Filter.BySlantAngle](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.BySlantAngle_img.jpg)

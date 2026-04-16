## Informacje szczegółowe
Ten węzeł wykonuje analizę odbić promienia w modelu programu Revit. Począwszy od danego punktu początkowego i przemieszczając się wzdłuż określonego wektora kierunku, węzeł śledzi ścieżkę promienia przecinającego elementy w modelu. Gdy promień pada na powierzchnię, może nadal odbijać się od tej powierzchni, w zależności od dozwolonej liczby odbić, symulując zachowanie światła, widoczność lub ścieżkę odbicia.

W tym przykładzie wybierany jest element, a jego położenie jest używane jako pozycja danych wejściowych origin dla węzła RayBounce.ByOriginDirection wraz z pozycjami direction, maxBounces i view.
___
## Plik przykładowy

![RayBounce.ByOriginDirection](./Revit.References.RayBounce.ByOriginDirection_img.jpg)

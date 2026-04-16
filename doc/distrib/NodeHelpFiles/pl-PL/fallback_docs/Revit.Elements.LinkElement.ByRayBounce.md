## Informacje szczegółowe
Ten węzeł rzuca promień na połączony model programu Revit z określonego początku i kierunku, a następnie śledzi jego kolejne odbicia od połączonych elementów. Każde odbicie reprezentuje punkt, w którym promień przecina geometrię w modelu połączonym, aż do zdefiniowanej maksymalnej liczby odbić.

W tym przykładzie wybieramy połączony element, a następnie używamy położenia tego elementu jako danych wejściowych origin dla węzła LinkElement.ByRayBounce wraz z danymi wejściowymi direction, maxBounces i view. Danymi wyjściowymi są punkty i elementy połączone.
___
## Plik przykładowy

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)

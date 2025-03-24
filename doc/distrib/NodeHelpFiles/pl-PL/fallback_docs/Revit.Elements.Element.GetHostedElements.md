## Informacje szczegółowe
Na podstawie elementu obsługującego elementy podrzędne _(np. ściany)_ węzeł `Element.GetHostedElements` zwraca elementy zależne od tego elementu. Domyślnie zwracane są wystąpienia rodzin, które są podrzędne wobec danego elementu. Węzeł `Element.GetHostedElements` udostępnia opcję dołączania innych typów elementów podrzędnych.

Należą do nich:
- otwory
- elementy podrzędne w połączonych elementach nadrzędnych
- ściany osadzone _(np. ściany osłonowe)_
- współdzielone osadzone elementy wstawiane

W poniższym przykładzie z poziomu L2 są pobierane wszystkie elementy ścian. Elementy ścian są dalej sprawdzane pod kątem elementów podrzędnych za pomocą węzła `Element.GetHostedElements`. Ta lista jest następnie używana do utworzenia dwóch list. Ściany z drzwiami i ściany bez drzwi.
___
## Plik przykładowy

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

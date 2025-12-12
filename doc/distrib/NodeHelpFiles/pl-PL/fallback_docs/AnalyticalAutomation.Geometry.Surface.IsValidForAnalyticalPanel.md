## Informacje szczegółowe
Ten węzeł ocenia daną powierzchnię w celu określenia, czy można jej użyć do zdefiniowania panelu analitycznego. Prawidłowa powierzchnia jest zazwyczaj płaska, ciągła i odpowiednia do przekształcenia w reprezentację analityczną w środowisku modelu analitycznego programu Revit.

W tym przykładzie pobierane są powierzchnie elementu płyty stropowej z projektu i górna powierzchnia jest przekazywana do węzła jako dane wejściowe. Węzeł zwraca wartość logiczną (Boolean) wskazującą, czy wybrana powierzchnia spełnia wymagania do utworzenia panelu analitycznego, wraz z opcjonalnym komunikatem opisującym wszelkie problemy napotkane podczas sprawdzania poprawności.
___
## Plik przykładowy

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)

## Informacje szczegółowe
Usuwa wszystkie zdefiniowane filtry z danej pozycji danych wejściowych ScheduleView.

W tym przykładzie początkowo zdefiniowaliśmy widok zestawienia, który jest kierowany bezpośrednio do węzła ScheduleView.ScheduleFilters, aby jawnie wyświetlić istniejące wcześniej filtry widoku. Aby wyróżnić przekształcenie, ten sam widok przechodzi następnie przez krótką, 10-milisekundową przerwę. Po tym krótkim opóźnieniu widok przechodzi do węzła ScheduleView.ClearAllFilters, w którym usuwane są wszystkie zastosowane filtry. Na koniec zmodyfikowany widok jest kierowany z powrotem do innego węzła ScheduleView.ScheduleFilters, co jednoznacznie pokazuje, że jego lista filtrów stała się listą pustą, co potwierdza pomyślne wyczyszczenie wszystkich oryginalnych filtrów.
___
## Plik przykładowy

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

## Informacje szczegółowe
Węzeł `Room.IsInsideRoom` zwraca wartość logiczną wskazującą, czy dany punkt znajduje się w danym pomieszczeniu.

W poniższym przykładzie pobierane są wszystkie meble w bieżącym dokumencie programu Revit wraz ze wszystkimi pomieszczeniami. Punkty położenia mebli są następnie przekazywane do węzła `Room.IsInsideRoom`, aby sprawdzić, w którym pomieszczeniu występują poszczególne punkty (jeśli są dostępne). Na koniec meble są filtrowane pod kątem elementów z lokalizacjami pomieszczeń i wartości te są łączone w listy.
___
## Plik przykładowy

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

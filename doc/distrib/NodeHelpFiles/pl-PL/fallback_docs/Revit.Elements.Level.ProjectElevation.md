## Informacje szczegółowe
Węzeł `Level.ProjectElevation` zwraca rzędną dla danego poziomu w jednostkach projektu. Węzeł `Level.ProjectElevation` zgłasza wartość od punktu początkowego projektu. Jeśli jest wymagana rzędna od poziomu gruntu, wartość tę można pobrać za pomocą węzła `Level.Elevation`.

W poniższym przykładzie zbierane są wszystkie poziomy w bieżącym dokumencie programu Revit. Zwracane są wartości rzędnych projektu poziomów. Ponadto poziomy są sortowane według wysokości za pomocą węzła `List.SortByKey`.
___
## Plik przykładowy

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)

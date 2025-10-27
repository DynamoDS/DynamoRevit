## Im Detail
Entfernt alle definierten Filter aus einer bestimmten Eingabe eines ScheduleView-Objekts.

In diesem Beispiel wurde zunächst eine Bauteillistenansicht definiert, die direkt an ScheduleView.ScheduleFilters weitergeleitet wird, um die bereits vorhandenen Ansichtsfilter explizit anzuzeigen. Zum Hervorheben der Transformation durchläuft dieselbe Ansicht dann eine kurze Unterbrechung von 10 Millisekunden. Nach dieser kurzen Verzögerung wechselt die Ansicht zum Block ScheduleView.ClearAllFilters, der alle angewendeten Filter entfernt. Schließlich wird die jetzt geänderte Ansicht zurück in einen weiteren ScheduleView.ScheduleFilters-Block geleitet, wodurch eindeutig demonstriert wird, dass die zugehörige Filterliste zu einer leeren Liste geworden ist, was das erfolgreiche Löschen aller ursprünglichen Filter bestätigt.
___
## Beispieldatei

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

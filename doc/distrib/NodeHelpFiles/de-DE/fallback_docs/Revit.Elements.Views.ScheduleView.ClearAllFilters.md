## Im Detail
Entfernt alle definierten Filter aus einer bestimmten Eingabe eines ScheduleView-Objekts.

In diesem Beispiel haben wir zunächst eine Bauteillisten-Ansicht definiert, die direkt an ScheduleView.ScheduleFilters weitergeleitet wird, um die bereits vorhandenen Ansichtsfilter explizit anzuzeigen. Um die Transformation hervorzuheben, durchläuft dieselbe Ansicht dann eine kurze Pause von 10 Millisekunden. Nach dieser kurzen Verzögerung wechselt die Ansicht zum Block ScheduleView.ClearAllFilters, der alle angewendeten Filter entfernt. Schließlich wird die jetzt geänderte Ansicht wieder in einen anderen ScheduleView.ScheduleFilters-Block geleitet, wodurch eindeutig demonstriert wird, dass die zugehörige Filterliste zu einer leeren Liste geworden ist, wodurch das erfolgreiche Löschen aller ursprünglichen Filter bestätigt wird.
___
## Beispieldatei

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

## In profondità
Rimuove tutti i filtri definiti da un determinato input di un elemento ScheduleView.

In questo esempio, inizialmente è stata definita una vista abaco che viene indirizzata direttamente a ScheduleView.ScheduleFilters per mostrare esplicitamente i filtri della vista preesistenti. Per evidenziare la trasformazione, questa stessa vista subisce poi una breve pausa di 10 millisecondi. Dopo questo breve ritardo, la vista passa al nodo ScheduleView.ClearAllFilters, che rimuove tutti i filtri applicati. Infine, la vista appena modificata viene incanalata nuovamente in un altro nodo ScheduleView.ScheduleFilters, dimostrando in modo inequivocabile che il relativo elenco di filtri è diventato un elenco vuoto e confermando così la corretta cancellazione di tutti i filtri originali.
___
## File di esempio

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

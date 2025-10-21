## Podrobnosti
Odebere všechny definované filtry z daného vstupu objektu ScheduleView.

V tomto příkladu jsme nejprve definovali pohled výkazu, který je směrován přímo do uzlu ScheduleView.ScheduleFilters, aby se explicitně zobrazily jeho dříve existující filtry pohledů. Aby se transformace zvýraznila, stejný pohled pak prochází krátkou 10milisekundovou pauzou. Po uplynutí této krátké prodlevy pohled pokračuje do uzlu ScheduleView.ClearAllFilters, který odebere všechny použité filtry. Nakonec je nyní upravený pohled směrován zpět do jiného uzlu ScheduleView.ScheduleFilters, čímž se jednoznačně prokáže, že se jeho seznam filtrů změnil na prázdný seznam a potvrdí se úspěšné vymazání všech původních filtrů.
___
## Vzorový soubor

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)

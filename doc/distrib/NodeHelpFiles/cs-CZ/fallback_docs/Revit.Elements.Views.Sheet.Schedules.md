## Podrobnosti
Uzel `Sheet.Schedules` vrací instance výkazů umístěné na daném výkresu. Poznámka: Uzel `Sheet.Schedules` vrátí revizní výkazy, které existují v rodině rohových razítek pro daný výkres.

V níže uvedeném příkladu jsou vráceny instance výkazů pro vybraný výkres. Revizní výkazy jsou poté odfiltrovány pomocí uzlu `List.FilterByBoolMask`. Kromě toho je načten pohled vlastníka instance výkazu pomocí uzlu `ScheduleOnSheet.ScheduleView`.
___
## Vzorový soubor

![Sheet.Schedules](./Revit.Elements.Views.Sheet.Schedules_img.jpg)

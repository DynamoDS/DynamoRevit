## Podrobnosti
Uzel `ScheduleFilter.FilterType` vrací metodu použitou pro vstupní filtr.
Mezi možné typy filtrů patří:

- RovnáSe – hodnota pole je rovna zadané hodnotě.
- NerovnáSe – hodnota pole není rovna zadané hodnotě.
- VětšíNež – hodnota pole je větší než zadaná hodnota.
- VětšíNeboRovno – hodnota pole je větší nebo rovna zadané hodnotě.
- MenšíNež – hodnota pole je menší než zadaná hodnota.
- MenšíNeboRovno – hodnota pole je menší nebo rovna zadané hodnotě.
- Obsahuje – u pole řetězce obsahuje hodnota pole zadaný řetězec.
- Neobsahuje – u pole řetězce neobsahuje hodnota pole zadaný řetězec.
- ZačínáNa – u pole řetězce začíná hodnota pole zadaným řetězcem.
- NezačínáNa – u pole řetězce nezačíná hodnota pole zadaným řetězcem.
- Končí – u pole řetězce končí hodnota pole zadaným řetězcem.
- Nekončí – u pole řetězce nekončí hodnota pole zadaným řetězcem.
- JeAsociovánoSGlobálnímParametrem – pole je asociováno s určeným globálním parametrem kompatibilního typu.
- NeníAsociovánoSGlobálnímParametrem – pole není asociováno s určeným globálním parametrem kompatibilního typu.

V níže uvedeném příkladu je načten první výkaz z aktuálního souboru aplikace Revit. V pohledu výkazu jsou poté vyhledány filtry a jediným použitým filtrem je typ filtru „řetězec nekončí“.
___
## Vzorový soubor

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)

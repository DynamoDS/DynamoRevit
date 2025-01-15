## Podrobnosti
Uzel `Level.ProjectElevation` vrací výšku daného podlaží v jednotkách projektu. Uzel `Level.ProjectElevation` vykáže hodnotu z počátku projektu. Pokud je vyžadována výška od úrovně terénu, lze tuto hodnotu získat pomocí uzlu `Level.Elevation`.

V níže uvedeném příkladu jsou z aktuálního dokumentu aplikace Revit shromážděna všechna podlaží. Je vrácena hodnota výšky projektu podlaží. Podlaží jsou navíc seřazena podle výšky pomocí uzlu `List.SortByKey`.
___
## Vzorový soubor

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)

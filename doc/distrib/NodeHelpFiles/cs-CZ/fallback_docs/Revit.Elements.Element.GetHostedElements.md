## Podrobnosti
Uzel `Element.GetHostedElements` vrací za předpokladu, že je dán prvek, který podporuje hostování prvků _(například stěny)_, prvky, které na daném prvku závisí. Ve výchozím nastavení jsou vráceny instance rodiny, které jsou hostovány do prvku. Uzel `Element.GetHostedElements` nabízí možnost zahrnout další typy hostovaných prvků.

Mezi tyto prvky patří:
- otvory
- prvky, které jsou hostovány ve spojených hostitelích
- vnořené stěny _(tj. obvodové pláště)_
- sdílené vnořené vložky

V níže uvedeném příkladu jsou shromážděny všechny prvky stěn z podlaží L2. U prvků stěn se poté zkontrolují hostované prvky pomocí uzlu `Element.GetHostedElements`. Pomocí tohoto seznamu se poté vytvoří dva seznamy. Stěny s dveřmi a stěny bez dveří.
___
## Vzorový soubor

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)

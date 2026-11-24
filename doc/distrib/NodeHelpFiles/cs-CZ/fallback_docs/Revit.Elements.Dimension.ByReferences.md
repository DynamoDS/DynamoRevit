## Podrobnosti
Uzel `Dimension.ByReferences` umístí kótu podél vstupní čáry ve vybraném pohledu pomocí daných referencí. Výstupem je prvek kóty aplikace Revit.

V níže uvedeném příkladu je vytvořeno několik bodů a jsou propojeny s uzly čar za účelem vytvoření křivek modelu v aplikaci Revit. Křivky modelu jsou poté převedeny na reference křivek aplikace Revit. Reference jsou sloučeny do seznamu a použijí se jako vstup `references` pro uzel `Dimension.ByReferences`. Vytvoří se další čáry, které definují umístění a směr kót. Aktivní pohled aplikace Revit je také zadán jako vstup `view`.
___
## Vzorový soubor

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)

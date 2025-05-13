## Podrobnosti
Uzel `Room.IsInsideRoom` vrací booleovskou hodnotu udávající, zda se daný bod v dané místnosti nachází, či nikoli.

V níže uvedeném příkladu je v aktuálním dokumentu aplikace Revit shromážděn veškerý nábytek spolu se všemi místnostmi. Body umístění nábytku se poté předají do uzlu `Room.IsInsideRoom`, aby bylo možné zkontrolovat, ve které místnosti se dané body vyskytují (pokud jsou k dispozici). Nakonec se z nábytku vyfiltrují prvky s umístěním místností a tyto hodnoty se zkombinují do seznamů.
___
## Vzorový soubor

![Room.IsInsideRoom](./Revit.Elements.Room.IsInsideRoom_img.jpg)

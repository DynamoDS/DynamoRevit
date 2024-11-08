## Podrobnosti
Uzel `Element.Delete` funguje stejně jako možnost odstranění v rozhraní aplikace Revit. Odstraní vstupní prvek a všechny prvky, které na něm závisí.

Například odstranění stěny obsahující dveře odstraní i dveře. Výstup se skládá z vnořeného seznamu id prvků, které byly v důsledku toho odstraněny. Poznámka: Tento uzel se nejlépe používá v režimu ručního spuštění v aplikaci Dynamo.

V níže uvedeném příkladu jsou všechny instance rodiny „Tlačítko nápovědy“ odstraněny z aktuálního dokumentu (souboru).
___
## Vzorový soubor

![Element.Delete](./Revit.Elements.Element.Delete_img.jpg)

## Podrobnosti
Uzel `Remember` uloží hodnotu do souboru aplikace Dynamo, aby ji graf mohl později znovu načíst, i když je vstup null. Je užitečná k uchování dat mezi spuštěními, zachování stavu nebo odkazování na dříve uložené informace bez nutnosti jejich opakovaného vytváření.

V níže uvedeném příkladu je vybráno jedno podlaží a geometrie je extrahována a použita jako vstup pro uzel `Remember`. Uložený výsledek je pak k dispozici jako výstup pro opakované použití v grafu.
___
## Vzorový soubor

![Remember](./CoreNodeModels.Remember_img.jpg)

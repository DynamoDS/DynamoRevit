## Podrobnosti
Tento uzel označí prvky aplikace Revit s daným pohledem, prvkem, umístěním, vodorovností (pokud ne, bude popisek orientován podle prvku) a addLeader jako vstupy.

V tomto příkladu jsou vybrány dveře v pohledu „Studio Live Work: CoreB“.  Umístění těchto dveří je extrahováno a poté použito jako původní vstup do Tag.ByElementAndLocation společně s booleovskými hodnotami pro vodorovnost a addLeader.  Původní umístění je upraveno tak, aby umístění popisku nepřekrývalo přímo horní část prvku pomocí uzlu Tag.SetHeadLocation.

___
## Vzorový soubor

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)

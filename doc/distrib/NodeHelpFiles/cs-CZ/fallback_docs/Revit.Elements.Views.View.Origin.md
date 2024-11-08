## Podrobnosti
Každý pohled v aplikaci Revit má počátek. Uzel `View.Origin` vrací tuto hodnotu jako bod aplikace Dynamo. Podle dokumentace rozhraní API aplikace Revit „Počátek půdorysného pohledu není smysluplný“. S ohledem na to hodnota předávaná uzlem `View.Origin` závisí na koncovém uživateli.

V níže uvedeném příkladu je vrácen počátek aktivního pohledu a vybraného 3D pohledu.
___
## Vzorový soubor

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)

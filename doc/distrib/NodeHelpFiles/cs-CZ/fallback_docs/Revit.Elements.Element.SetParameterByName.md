## Podrobnosti
Uzel `Element.SetParameterByName` nastaví prvek parametru (nalezený podle názvu) na danou hodnotu. Hodnoty zahrnují typy: double, integer, boolean, ElementId, Element a string. V aplikaci Revit mohou parametry sdílet stejný název. Ve výsledku uzel `Element.SetParameterByName` nastaví hodnotu prvního nalezeného parametru, seřazeného abecedně podle hodnoty UniqueId.

V níže uvedeném příkladu je nastaven parametr komentářů pro všechny položky nábytku v modelu, které se nacházejí v místnosti. Hodnota parametru komentářů je název místnosti, který je získán.
___
## Vzorový soubor

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)

## Informacje szczegółowe
Węzeł `Element.Solids` zwraca geometrię bryły dla danego elementu. Bryły są zwracane jako listy zagnieżdżone, ponieważ każdy dany element może zawierać więcej niż jedną bryłę. Jeśli potrzebna jest pojedyncza bryła reprezentująca element, do list można zastosować węzeł `Solid.ByUnion`.

W poniższym przykładzie z wybranego widoku są pobierane wszystkie ściany. Ściany, które zostały utworzone jako rodziny lokalne, są następnie usuwane, i zwracane są pozostałe bryły ścian.

___
## Plik przykładowy

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

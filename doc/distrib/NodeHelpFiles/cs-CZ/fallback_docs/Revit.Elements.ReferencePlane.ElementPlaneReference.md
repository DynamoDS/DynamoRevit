## Podrobnosti
Tento uzel extrahuje skutečnou referenci na prvek aplikace Revit vybrané referenční roviny. To je užitečné, pokud potřebujete danou rovinu použít jako referenci hostitele pro geometrii nebo kóty v aplikaci Revit.

Příklad:
V tomto grafu jsou dva body definovány pomocí souřadnic a mezi nimi je pomocí metody ReferencePlane.ByStartPointEndPoint vytvořena referenční rovina. Tato referenční rovina je poté připojena k uzlu ReferencePlane.ElementPlaneReference, který je výstupem nativní reference roviny pro aplikaci Revit, čímž se připraví k použití pro úlohy hostování nebo zarovnání.
___
## Vzorový soubor

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)

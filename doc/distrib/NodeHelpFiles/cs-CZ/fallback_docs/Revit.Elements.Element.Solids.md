## Podrobnosti
Uzel `Element.Solids` vrací geometrii tělesa pro daný prvek. Tělesa jsou vrácena jako vnořené seznamy, protože každý daný prvek může mít v sobě více než jedno těleso. Pokud je požadováno jedno těleso, které představuje prvek, může se na seznamy použít uzel `Solid.ByUnion`.

V níže uvedeném příkladu jsou shromážděny všechny stěny z vybraného pohledu. Stěny vytvořené jako rodiny na místě jsou poté odebrány a jsou vrácena tělesa zbývajících stěn.

___
## Vzorový soubor

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)

## Podrobnosti
Uzel `Space.CenterBoundary` vrací vnořený seznam představující hranici osy daného prostoru. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco následující seznamy představují smyčky v prostoru. Středové hranice jsou umístěny na ose stěny napříč všemi hladinami v prostoru aplikace Revit. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadán neohraničený nebo neumístěný prostor, je vrácena hodnota null.

V níže uvedeném příkladu jsou z aktuálního dokumentu a vybraného pohledu shromážděny všechny prostory. Poté jsou vráceny hranice středu.
___
## Vzorový soubor

![Space.CenterBoundary](./Revit.Elements.Space.CenterBoundary_img.jpg)

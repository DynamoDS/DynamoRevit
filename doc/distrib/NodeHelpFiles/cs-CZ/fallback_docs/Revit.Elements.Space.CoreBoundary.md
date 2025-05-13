## Podrobnosti
Uzel `Space.CoreBoundary` vrací vnořený seznam představující hranici nosné části daného prostoru. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco následující seznamy představují smyčky v prostoru. Hranice nosné části jsou umístěny na vnitřní nebo vnější hladině nosné části, která je nejblíže místnosti. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadán neohraničený nebo neumístěný prostor, je vrácena hodnota null.

V níže uvedeném příkladu jsou z aktuálního dokumentu a vybraného pohledu shromážděny všechny prostory. Poté jsou vráceny hranice nosné části.

___
## Vzorový soubor

![Space.CoreBoundary](./Revit.Elements.Space.CoreBoundary_img.jpg)

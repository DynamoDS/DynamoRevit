## Podrobnosti
Uzel `Space.CoreCenterBoundary` vrací vnořený seznam představující hranici středu nosné části daného prostoru. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco další seznamy představují smyčky v prostoru. Hranice středu nosné části jsou umístěny ve středech stěn, které jsou definovány jako nosná část. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadán neohraničený nebo neumístěný prostor, je vrácena hodnota null.

V níže uvedeném příkladu jsou z aktuálního dokumentu a vybraného pohledu shromážděny všechny prostory. Poté jsou vráceny hranice středu nosné části.

___
## Vzorový soubor

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)

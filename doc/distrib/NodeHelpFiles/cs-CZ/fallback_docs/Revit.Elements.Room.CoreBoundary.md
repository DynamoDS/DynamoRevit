## Podrobnosti
Uzel `Room.CoreBoundary` vrací vnořený seznam představující hranici nosné části dané místnosti. Ve vráceném seznamu představuje první dílčí seznam vnější křivky, zatímco další dílčí seznamy představují smyčky uvnitř místnosti. Hranice nosné části jsou umístěny na vnitřní nebo vnější hladině vzhledem k nosné části, která je k místnosti nejblíže. Další informace o čarách umístění stěn naleznete v tomto [článku](https://help.autodesk.com/view/RVT/2024/CSY/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) nápovědy.

Pokud je zadána neohraničená nebo neumístěná místnost, je vrácena hodnota null.

V níže uvedeném příkladu jsou shromážděny všechny místnosti z aktuálního dokumentu a vybraného pohledu. Poté jsou vráceny hranice nosné části.
___
## Vzorový soubor

![Room.CoreBoundary](./Revit.Elements.Room.CoreBoundary_img.jpg)
